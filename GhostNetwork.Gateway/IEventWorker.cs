using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Events
{
    public interface IEventWorker
    {
        Task Subscribe<T>(RedisKey key) where T : IEvent, new();
    }
}
