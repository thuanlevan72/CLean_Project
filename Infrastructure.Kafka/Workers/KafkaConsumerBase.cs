using Confluent.Kafka;
using Infrastructure.Redis.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Kafka.Workers;

public abstract class KafkaConsumerBase<TMessage> : BackgroundService
{
    private readonly string _topic;
    private readonly IConsumer<string, string> _consumer;
    protected readonly ILogger _logger;
    private readonly IRedisMessageTracker _redisTracker;

    protected KafkaConsumerBase(string topic, string groupId, string bootstrapServers, ILogger logger, IRedisMessageTracker redisTracker)
    {
        _topic = topic;
        _logger = logger;
        _redisTracker = redisTracker;

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected abstract Task ProcessMessageAsync(TMessage message, string key, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("🎧 Bắt đầu lắng nghe Topic: {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result == null) return;

                    Guid.TryParse(result.Message.Key, out Guid messageId);

                    try
                    {
                        var message = JsonSerializer.Deserialize<TMessage>(result.Message.Value);
                        if (message != null)
                        {
                            // Gọi con xử lý nghiệp vụ
                            await ProcessMessageAsync(message, result.Message.Key, stoppingToken);
                        }

                        // Tự động báo thành công
                        if (messageId != Guid.Empty) await _redisTracker.LogProcessedAsync(messageId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Lỗi xử lý tin nhắn.");
                        // Tự động báo lỗi
                        if (messageId != Guid.Empty) await _redisTracker.LogProcessedAsync(messageId, ex.Message);
                    }
                    finally
                    {
                        _consumer.Commit(result);
                    }
                }, stoppingToken);
            }
            catch (OperationCanceledException) { }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        base.Dispose();
    }
}