using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Events
{
    public interface IEventMessageHandler
    {
        Task Subscribe<T>(RedisKey key) where T : BaseEvent;
    }
}
