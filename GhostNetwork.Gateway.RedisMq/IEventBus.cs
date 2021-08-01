using GhostNetwork.Gateway.RedisMq.Events;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventBus
    {
        Task Subscribe<T>(RedisKey key) where T : EventBase, new();
        Task PublishAsync(EventBase @event);
    }
}
