using System.Threading.Tasks;
using GhostNetwork.Gateway.RedisMq.Events;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventHandler 
    {
        Task Handle(EventBase value);
    }

    public interface IEventHandler<TEvent> : IEventHandler where TEvent : EventBase { }
}