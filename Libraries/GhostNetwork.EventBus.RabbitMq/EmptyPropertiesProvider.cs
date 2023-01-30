using RabbitMQ.Client;

namespace GhostNetwork.EventBus.RabbitMq;

public class EmptyPropertiesProvider : IPropertiesProvider
{
    public IBasicProperties GetProperties(IModel channel)
    {
        return channel.CreateBasicProperties();
    }
}