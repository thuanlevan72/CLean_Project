using Domain.Entities;
using Infrastructure.Postgres.Data;
using Infrastructure.Redis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Postgres.Workers;

public class RedisTrackingBatchWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RedisTrackingBatchWorker> _logger;
    private const string TRACKING_KEY = "kafka_message_tracking_buffer";
    private const int BATCH_SIZE = 5000;

    public RedisTrackingBatchWorker(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory, ILogger<RedisTrackingBatchWorker> logger)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 [BATCH WORKER] Bắt đầu gom lô Tracking từ Redis...");
        var db = _redis.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var redisItems = await db.ListRightPopAsync(TRACKING_KEY, BATCH_SIZE);

                if (redisItems != null && redisItems.Length > 0)
                {
                    await ProcessBatchAsync(redisItems, stoppingToken);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken); // Hết việc thì ngủ 5s
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi lúc gom lô.");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessBatchAsync(RedisValue[] redisItems, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var parsedMessages = redisItems
            .Select(x => JsonSerializer.Deserialize<RedisTrackedMessage>(x.ToString()))
            .Where(x => x != null)
            .ToList();

        // Gộp những hành động trùng MessageId
        var groupedMessages = parsedMessages.GroupBy(m => m!.MessageId).ToList();
        var messageIds = groupedMessages.Select(g => g.Key).ToList();

        var existingRecords = await dbContext.OutboxMessages
            .Where(x => messageIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, ct);

        var newInserts = new List<OutboxMessage>();
        var updates = new List<OutboxMessage>();

        foreach (var group in groupedMessages)
        {
            var id = group.Key;
            var logs = group.OrderBy(m => m!.TimestampUtc).ToList();

            existingRecords.TryGetValue(id, out var dbRecord);

            if (dbRecord == null)
            {
                dbRecord = new OutboxMessage { Id = id };
                newInserts.Add(dbRecord);
            }
            else
            {
                updates.Add(dbRecord);
            }

            foreach (var log in logs)
            {
                if (log!.Status == "Sent")
                {
                    dbRecord.Topic = log.Topic;
                    dbRecord.Payload = log.Payload;
                    dbRecord.CreatedOnUtc = log.TimestampUtc;
                }
                else if (log.Status == "Processed") dbRecord.ProcessedOnUtc = log.TimestampUtc;
                else if (log.Status == "Failed") dbRecord.Error = log.Error;
            }
        }

        // Bắn tốc độ cao xuống Postgres
        if (newInserts.Any()) dbContext.OutboxMessages.AddRange(newInserts);
        if (updates.Any()) dbContext.OutboxMessages.UpdateRange(updates);

        await dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("📦 Đã gom và lưu {Count} hành động Kafka xuống Database.", redisItems.Length);
    }
}