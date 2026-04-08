using Domain.Entities;
using Infrastructure.Redis.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using Infrastructure.Postgres.Data;

namespace Infrastructure.Hangfire.Jobs;

public class RedisTrackingBatchJob
{
    private readonly IConnectionMultiplexer _redis;
    private readonly AppDbContext _dbContext; // Inject thẳng DbContext cực nhàn!
    private readonly ILogger<RedisTrackingBatchJob> _logger;
    private const string TRACKING_KEY = "kafka_message_tracking_buffer";

    public RedisTrackingBatchJob(IConnectionMultiplexer redis, AppDbContext dbContext, ILogger<RedisTrackingBatchJob> logger)
    {
        _redis = redis;
        _dbContext = dbContext;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessRedisBatchAsync()
    {
        var db = _redis.GetDatabase();
        var redisItems = await db.ListRightPopAsync(TRACKING_KEY, 5000);

       if (redisItems != null && redisItems.Length > 0)
        {
            _logger.LogInformation("🚀 [Hangfire] Đang gom {Count} tin từ Redis xuống Postgres...", redisItems.Length);

            var parsedMessages = redisItems
                .Select(x => JsonSerializer.Deserialize<RedisTrackedMessage>(x.ToString()))
                .Where(x => x != null)
                .ToList();

            var groupedMessages = parsedMessages.GroupBy(m => m!.MessageId).ToList();
            var messageIds = groupedMessages.Select(g => g.Key).ToList();

            var existingRecords = await _dbContext.OutboxMessages
                .Where(x => messageIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

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
                else updates.Add(dbRecord);

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

            if (newInserts.Any()) _dbContext.OutboxMessages.AddRange(newInserts);
            if (updates.Any()) _dbContext.OutboxMessages.UpdateRange(updates);

            await _dbContext.SaveChangesAsync();
        }
    }
}