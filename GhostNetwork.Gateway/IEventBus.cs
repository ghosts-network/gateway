using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Events
{
    public interface IEventBus
    {
        Task Subscribe<T>(RedisKey key) where T : IEvent, new();
        Task PublishAsync(IEvent @event);
    }
}
