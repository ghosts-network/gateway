using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway
{
    public interface IEventWorker
    {
        Task Handle<T>(RedisValue value) where T : BaseEvent;
    }
}
