using System;
using Azure.Messaging.ServiceBus;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public interface IMessageProvider
    {
        ServiceBusMessage GetMessage<TEvent>(TEvent @event)
            where TEvent : Event;

        object GetEvent(byte[] message, Type outputType);
    }
}