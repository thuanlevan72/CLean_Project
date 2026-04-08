using Application.Interfaces; // Chỉ dùng cho IKafkaPublisher // <--- CẬP NHẬT DÒNG NÀY (Thay cho Application.Interfaces cũ)
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka.Workers;

public class OutboxRelayWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxRelayWorker> _logger;

    public OutboxRelayWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxRelayWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Outbox Relay Worker đã khởi động...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                // Xin ra đúng Interface từ thư mục Domain
                var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var kafkaPublisher = scope.ServiceProvider.GetRequiredService<IKafkaPublisher>();

                var messages = await outboxRepo.GetUnprocessedMessagesAsync(50, stoppingToken);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        try
                        {
                            await kafkaPublisher.PublishAsync(message.Topic, message.Payload, stoppingToken);
                            message.ProcessedOnUtc = DateTime.UtcNow;
                            message.Error = null;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi Kafka khi gửi Outbox ID: {Id}", message.Id);
                            message.Error = ex.Message;
                        }

                        await outboxRepo.UpdateMessageAsync(message, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi hệ thống quét bảng Outbox.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}