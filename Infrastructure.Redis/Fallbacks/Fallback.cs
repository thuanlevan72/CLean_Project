using Application.Interfaces;
using Infrastructure.Redis.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Redis.Fallbacks
{
    public class FallbackRateLimitService : IRateLimitService
    {
        // Khi sập Redis, tạm thời tắt tính năng chống Spam (cho phép qua hết)
        public Task<bool> IsAllowedAsync(string key, int maxRequests, TimeSpan window)
            => Task.FromResult(true);
    }

    public class FallbackDistributedLockService : IDistributedLockService
    {
        // Khi sập Redis, tạm tắt tính năng Lock phân tán (chấp nhận rủi ro nhỏ)
        public Task<bool> AcquireLockAsync(string resourceKey, string token, TimeSpan expiry)
            => Task.FromResult(true);

        public Task ReleaseLockAsync(string resourceKey, string token)
            => Task.CompletedTask;
    }

    public class FallbackPubSubService : IPubSubService
    {
        // PubSub không có Redis sẽ không chạy được trên nhiều server, tạm vô hiệu hóa
        public Task PublishAsync(string channel, string message)
            => Task.CompletedTask;

        public Task SubscribeAsync(string channel, Action<string, string> handler)
            => Task.CompletedTask;
    }

    public class FallbackRedisMessageTracker : IRedisMessageTracker
    {

        Task IRedisMessageTracker.LogSentAsync(Guid messageId, string topic, object payload) => Task.CompletedTask;

        Task IRedisMessageTracker.LogProcessedAsync(Guid messageId, string? error) => Task.CompletedTask;
    }
}
