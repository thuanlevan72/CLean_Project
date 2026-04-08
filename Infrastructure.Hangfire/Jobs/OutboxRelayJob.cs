using Application.Interfaces;
using Domain.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Hangfire.Jobs;

public class OutboxRelayJob
{
    private readonly IOutboxRepository _outboxRepo;
    private readonly IKafkaPublisher _kafkaPublisher;
    private readonly ILogger<OutboxRelayJob> _logger;

    // 1. Tiêm TRỰC TIẾP các Interface vào. Hangfire sẽ tự động quản lý Scope cực kỳ an toàn!
    public OutboxRelayJob(
        IOutboxRepository outboxRepo,
        IKafkaPublisher kafkaPublisher,
        ILogger<OutboxRelayJob> logger)
    {
        _outboxRepo = outboxRepo;
        _kafkaPublisher = kafkaPublisher;
        _logger = logger;
    }

    // Hangfire sẽ tự động gọi lại hàm này tối đa 3 lần nếu có lỗi văng ra (Crash)
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessOutboxAsync()
    {
        // 2. KHÔNG CÒN VÒNG LẶP WHILE. Hangfire gọi hàm này -> Chạy từ trên xuống dưới -> Xong là tắt.
        var messages = await _outboxRepo.GetUnprocessedMessagesAsync(50);

        if (!messages.Any())
        {
            // Nếu DB không có thư nào thì kết thúc Job cho nhẹ máy.
            return;
        }

        _logger.LogInformation("📦 [Hangfire] Đang xử lý {Count} tin nhắn trong Outbox...", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                // Bắn lên Kafka
                await _kafkaPublisher.PublishAsync(message.Topic, message.Payload);

                // Đánh dấu thành công
                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                // Nếu Kafka sập mạng, ghi lỗi lại để lần sau quét tiếp
                _logger.LogError(ex, "❌ Lỗi Kafka khi gửi Outbox ID: {Id}", message.Id);
                message.Error = ex.Message;
            }

            // 3. Update trạng thái (Thành công hay Thất bại đều lưu)
            await _outboxRepo.UpdateMessageAsync(message);
        }
    }
}