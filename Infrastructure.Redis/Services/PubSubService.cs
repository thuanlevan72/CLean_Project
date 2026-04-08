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
        await sub.PublishAsync(channel, message);
    }

    public async Task SubscribeAsync(string channel, Action<string, string> handler)
    {
        var sub = _redis.GetSubscriber();
        await sub.SubscribeAsync(channel, (ch, msg) =>
        {
            handler(ch.ToString(), msg.ToString());
        });
    }
}