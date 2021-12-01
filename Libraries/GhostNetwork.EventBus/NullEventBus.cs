using System.Threading.Tasks;

namespace GhostNetwork.EventBus
{
    public class NullEventBus : IEventBus
    {
        public Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : Event
        {
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
        }
    }
}