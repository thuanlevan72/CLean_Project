using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Redis.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;

    public RateLimitService(IConnectionMultiplexer redis) { _redis = redis; }

    public async Task<bool> IsAllowedAsync(string key, int maxRequests, TimeSpan window)
    {
        var db = _redis.GetDatabase();
        var rateKey = $"ratelimit:{key}";

        // Tăng biến đếm lên 1. Nếu key chưa tồn tại, Redis tự tạo và gán = 1
        long requests = await db.StringIncrementAsync(rateKey);

        // Nếu là request đầu tiên, set thời gian hết hạn (VD: 1 phút)
        if (requests == 1)
        {
            await db.KeyExpireAsync(rateKey, window);
        }

        return requests <= maxRequests;
    }
}