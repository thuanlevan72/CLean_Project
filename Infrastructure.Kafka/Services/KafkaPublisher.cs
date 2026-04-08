using Application.Interfaces;
using Confluent.Kafka;
using Infrastructure.Redis.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Kafka.Services;

public class KafkaPublisher : IKafkaPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly IRedisMessageTracker _redisTracker;

    public KafkaPublisher(IConfiguration configuration, IRedisMessageTracker redisTracker)
    {
        _redisTracker = redisTracker;
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Acks = Acks.All,
            EnableIdempotence = true
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public Task PublishAsync<TValue>(string topic, TValue message, CancellationToken ct = default)
    {
        return PublishWithKeyAsync(topic, Guid.NewGuid().ToString(), message, ct);
    }

    public async Task PublishWithKeyAsync<TKey, TValue>(string topic, TKey key, TValue message, CancellationToken ct = default)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = key?.ToString() ?? Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(message)
        };

        // 1. Gửi lên Kafka
        await _producer.ProduceAsync(topic, kafkaMessage, ct);

        // 2. Báo cáo siêu tốc vào Redis
        if (Guid.TryParse(kafkaMessage.Key, out Guid messageId))
        {
            await _redisTracker.LogSentAsync(messageId, topic, message!);
        }
    }

    public void Dispose() => _producer.Dispose();
}