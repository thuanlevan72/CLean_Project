using Application.Events;
using Infrastructure.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka.Workers;

public class AuditLogWorker : KafkaConsumerBase<TodoCreatedEvent>
{
    public AuditLogWorker(IConfiguration config, ILogger<AuditLogWorker> logger, IRedisMessageTracker redisTracker)
        : base("todo-events", "audit_log_group", config["Kafka:BootstrapServers"]!, logger, redisTracker)
    {
    }

    protected override async Task ProcessMessageAsync(TodoCreatedEvent message, string key, CancellationToken ct)
    {
        _logger.LogInformation("✅ [AUDIT WORKER] Đang ghi log hành động: {Title}", message.Title);
        await Task.Delay(100, ct); // Giả lập chạy
    }
}