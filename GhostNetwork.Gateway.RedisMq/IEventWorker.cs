using GhostNetwork.Gateway.RedisMq.Events;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventWorker
    {
        Task Subscribe<T>(RedisKey key) where T : EventBase, new();
    }
}
