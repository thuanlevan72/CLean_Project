using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Redis.Services;

public interface IRedisBufferService
{
    // Hàm này bạn có thể gọi ở bất cứ đâu (lúc API nhận request, hoặc lúc Consumer nhận tin)
    Task PushToBufferAsync(string topic, object payload);
}

public class RedisBufferService : IRedisBufferService
{
    private readonly IConnectionMultiplexer _redis;
    private const string BUFFER_KEY = "system_message_buffer";

    public RedisBufferService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task PushToBufferAsync(string topic, object payload)
    {
        var db = _redis.GetDatabase();

        // Đóng gói dữ liệu kèm thời gian thực
        var bufferItem = new
        {
            Topic = topic,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow
        };

        var jsonStr = JsonSerializer.Serialize(bufferItem);

        // Nhét vào đầu danh sách (List) của Redis. Thao tác này mất < 1 mili-giây!
        await db.ListLeftPushAsync(BUFFER_KEY, jsonStr);
    }
}