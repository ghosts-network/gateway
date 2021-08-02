using System.Threading.Tasks;
using GhostNetwork.Gateway.RedisMq.Events;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventSender
    {
        Task PublishAsync(EventBase @event);
    }
}