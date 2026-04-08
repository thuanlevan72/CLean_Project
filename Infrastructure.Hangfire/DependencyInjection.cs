using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Hangfire.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Hangfire;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        return services;
    }

    public static IServiceCollection AddHangfireServerAndJobs(this IServiceCollection services)
    {
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 10;
        });

        return services;
    }

    // ✨ THÊM MỚI: Giấu logic lập lịch vào đây để Program.cs được sạch
    public static void InitializeHangfireJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobs.AddOrUpdate<RedisTrackingBatchJob>(
            "redis-batch-job",
            job => job.ProcessRedisBatchAsync(),
            Cron.Minutely());
    }
}