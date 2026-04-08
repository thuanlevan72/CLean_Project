using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Redis.Services;

public class DistributedLockService : IDistributedLockService
{
    private readonly IConnectionMultiplexer _redis;

    public DistributedLockService(IConnectionMultiplexer redis) { _redis = redis; }

    public async Task<bool> AcquireLockAsync(string resourceKey, string token, TimeSpan expiry)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock:{resourceKey}";

        // Mấu chốt: When.NotExists (NX). 
        // Lệnh này đảm bảo tính nguyên tử (Atomic). Chỉ ai đến NHANH NHẤT mới lấy được cờ (trả về true).
        return await db.StringSetAsync(lockKey, token, expiry, When.NotExists);
    }

    public async Task ReleaseLockAsync(string resourceKey, string token)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock:{resourceKey}";

        // Chỉ người nào cầm đúng Token mới được mở khóa (dùng Lua Script để đảm bảo Atomic)
        var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        await db.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { token });
    }
}