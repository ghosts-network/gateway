using System.Threading.Tasks;

namespace GhostNetwork.EventBus
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task ProcessAsync(TEvent @event);
    }
}