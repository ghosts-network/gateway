using System;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public interface IHandlerProvider
    {
        IEventHandler<TEvent> GetRequiredService<TEvent>(Type handlerType)
            where TEvent : Event;
    }
}
