using System;
using System.Text;
using System.Text.Json;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public class JsonMessageProvider : IMessageProvider
    {
        public byte[] GetMessage<TEvent>(TEvent @event)
            where TEvent : Event
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        }

        public object GetEvent(byte[] message, Type outputType)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(message), outputType);
        }
    }
}