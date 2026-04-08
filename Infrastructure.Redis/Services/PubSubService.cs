using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Redis.Services;

public class PubSubService : IPubSubService
{
    private readonly IConnectionMultiplexer _redis;

    public PubSubService(IConnectionMultiplexer redis) { _redis = redis; }

    public async Task PublishAsync(string channel, string message)
    {
        var sub = _redis.GetSubscriber();
        await sub.PublishAsync(RedisChannel.Literal("channel"), message);   
    }

    public async Task SubscribeAsync(string channel, Action<string, string> handler)
    {
        var sub = _redis.GetSubscriber();
        await sub.SubscribeAsync(RedisChannel.Literal("channel"), (ch, msg) =>
        {
            handler(ch.ToString(), msg.ToString());
        });
    }
}