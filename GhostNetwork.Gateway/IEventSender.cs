using GhostNetwork.Gateway.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway
{
    public interface IEventSender
    {
        Task PublishAsync(IEvent @event);
    }
}
