using GhostNetwork.Gateway.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway
{
    public interface IEventMessageSender
    {
        Task PublishAsync<T>(T @event) where T : BaseEvent;
    }
}
