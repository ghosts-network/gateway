using System;

namespace GhostNetwork.EventBus.RabbitMq
{
    public interface IMessageProvider
    {
        byte[] GetMessage<TEvent>(TEvent @event)
            where TEvent : Event;

        object GetEvent(byte[] message, Type outputType);
    }
}