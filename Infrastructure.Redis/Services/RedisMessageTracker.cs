using Infrastructure.Redis.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Redis.Services;

public interface IRedisMessageTracker
{
    Task LogSentAsync(Guid messageId, string topic, object payload);
    Task LogProcessedAsync(Guid messageId, string? error = null);
}

public class RedisMessageTracker : IRedisMessageTracker
{
    private readonly IConnectionMultiplexer _redis;
    private const string TRACKING_KEY = "kafka_message_tracking_buffer";

    public RedisMessageTracker(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task LogSentAsync(Guid messageId, string topic, object payload)
    {
        var msg = new RedisTrackedMessage
        {
            MessageId = messageId,
            Topic = topic,
            Payload = JsonSerializer.Serialize(payload),
            Status = "Sent",
            TimestampUtc = DateTime.UtcNow
        };
        await _redis.GetDatabase().ListLeftPushAsync(TRACKING_KEY, JsonSerializer.Serialize(msg));
    }

    public async Task LogProcessedAsync(Guid messageId, string? error = null)
    {
        var msg = new RedisTrackedMessage
        {
            MessageId = messageId,
            Status = string.IsNullOrEmpty(error) ? "Processed" : "Failed",
            TimestampUtc = DateTime.UtcNow,
            Error = error
        };
        await _redis.GetDatabase().ListLeftPushAsync(TRACKING_KEY, JsonSerializer.Serialize(msg));
    }
}