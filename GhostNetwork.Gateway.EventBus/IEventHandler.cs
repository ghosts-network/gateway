using System.Threading.Tasks;
using GhostNetwork.Gateway.EventBus.Events;

namespace GhostNetwork.Gateway.EventBus
{
    public interface IEventHandler 
    {

        Task Handle(EventBase value);
    }

    public interface IEventHandler<TEvent> : IEventHandler where TEvent : EventBase 
    {
        Task Handle(TEvent value);
    }
}