using Application.Interfaces;
using Infrastructure.Redis.Fallbacks;
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
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = true;
                options.ConnectTimeout = 3000;

                var multiplexer = ConnectionMultiplexer.Connect(options);

                services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                services.AddStackExchangeRedisCache(opt => { opt.Configuration = redisConnectionString; });

                services.AddScoped<IRateLimitService, RateLimitService>();
                services.AddScoped<IDistributedLockService, DistributedLockService>();
                services.AddScoped<IPubSubService, PubSubService>();

                // 🔥 FIX QUAN TRỌNG: Phải là Singleton để đi chung với KafkaPublisher
                services.AddSingleton<IRedisMessageTracker, RedisMessageTracker>();

                isRedisConnected = true;
                Console.WriteLine("✅ Đã kết nối Redis Server thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kết nối Redis thất bại: {ex.Message}");
                Console.WriteLine("⚠️ Đang kích hoạt chế độ Cache dự phòng (In-Memory)...");
            }
        }

        if (!isRedisConnected)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<IRateLimitService, FallbackRateLimitService>();
            services.AddScoped<IDistributedLockService, FallbackDistributedLockService>();
            services.AddScoped<IPubSubService, FallbackPubSubService>();

            // 🔥 Cũng phải đổi thành Singleton
            services.AddSingleton<IRedisMessageTracker, FallbackRedisMessageTracker>();
        }

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}

// Lời khuyên: Các class Fallback này nên được bôi đen, Cắt (Cut) và Dán sang 
// các file riêng biệt trong thư mục `Infrastructure.Redis/Fallbacks/` để file này thật gọn.