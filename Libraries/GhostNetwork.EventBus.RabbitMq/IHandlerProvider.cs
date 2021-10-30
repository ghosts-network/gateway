using System;

namespace GhostNetwork.EventBus.RabbitMq
{
    public interface IHandlerProvider
    {
        IEventHandler<TEvent> GetRequiredService<TEvent>(Type handlerType)
            where TEvent : Event;
    }
}
