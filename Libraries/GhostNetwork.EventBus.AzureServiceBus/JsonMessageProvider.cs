using System;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public class JsonMessageProvider : IMessageProvider
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        public ServiceBusMessage GetMessage<TEvent>(TEvent @event)
            where TEvent : Event
        {
            return new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, Options)));
        }

        public object GetEvent(byte[] message, Type outputType)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(message), outputType, Options);
        }
    }
}