using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway
{
    public interface IEventHandler { }
    public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent, new() 
    {
        Task Handle(TEvent value);
    }
}
