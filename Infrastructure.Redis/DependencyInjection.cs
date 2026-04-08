using Application.Interfaces;
using Infrastructure.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnection");
        bool isRedisConnected = false;

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            try
            {
                // Cấu hình Timeout ngắn (3 giây) để app không bị treo lâu nếu Redis sập
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = true;
                options.ConnectTimeout = 3000;

                // THỬ KẾT NỐI
                var multiplexer = ConnectionMultiplexer.Connect(options);

                // NẾU THÀNH CÔNG -> Đăng ký đồ thật
                services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                services.AddStackExchangeRedisCache(opt => { opt.Configuration = redisConnectionString; });

                services.AddScoped<IRateLimitService, RateLimitService>();
                services.AddScoped<IDistributedLockService, DistributedLockService>();
                services.AddScoped<IPubSubService, PubSubService>();

                isRedisConnected = true;
                Console.WriteLine("✅ Đã kết nối Redis Server thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kết nối Redis thất bại: {ex.Message}");
                Console.WriteLine("⚠️ Đang kích hoạt chế độ Cache dự phòng (In-Memory)...");
            }
        }

        // ==============================================================
        // CHẾ ĐỘ DỰ PHÒNG (FALLBACK) KHÍ REDIS CHẾT HOẶC CHƯA CÀI ĐẶT
        // ==============================================================
        if (!isRedisConnected)
        {
            // 1. Chuyển Cache về dùng RAM của máy chủ chạy API
            services.AddDistributedMemoryCache();

            // 2. Chuyển các Service phụ thuộc Redis về chế độ "Mock" để không crash App
            services.AddScoped<IRateLimitService, FallbackRateLimitService>();
            services.AddScoped<IDistributedLockService, FallbackDistributedLockService>();
            services.AddScoped<IPubSubService, FallbackPubSubService>();
        }

        // Service này dùng IDistributedCache (interface chung) nên Redis hay RAM nó đều chạy tốt!
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}

// =========================================================================
// CÁC CLASS DỰ PHÒNG (NO-OP/MOCK) - Đặt luôn ở dưới file này cho gọn
// =========================================================================

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