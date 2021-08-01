using System.Threading.Tasks;
using GhostNetwork.Gateway.RedisMq.Events;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventHandler<TEvent> where TEvent : EventBase, new() 
    {
        Task Handle(TEvent value);
    }
}
