using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka.Fallbacks;

public class FallbackKafkaPublisher : IKafkaPublisher
{
    private readonly ILogger<FallbackKafkaPublisher> _logger;

    public FallbackKafkaPublisher(ILogger<FallbackKafkaPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TValue>(string topic, TValue message, CancellationToken ct = default)
    {
        _logger.LogWarning("⚠️ [FALLBACK] Kafka đang sập. Bỏ qua gửi tin nhắn lên Topic: {Topic}", topic);
        return Task.CompletedTask;
    }

    public Task PublishWithKeyAsync<TKey, TValue>(string topic, TKey key, TValue message, CancellationToken ct = default)
    {
        _logger.LogWarning("⚠️ [FALLBACK] Kafka đang sập. Bỏ qua gửi tin nhắn (Key: {Key}) lên Topic: {Topic}", key, topic);
        return Task.CompletedTask;
    }
}