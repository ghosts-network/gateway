using System.Threading.Tasks;
using GhostNetwork.Gateway.EventBus.Events;

namespace GhostNetwork.Gateway.EventBus
{
    public interface IEventSender
    {
        Task PublishAsync(EventBase @event);
    }
}