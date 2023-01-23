using RabbitMQ.Client;

namespace GhostNetwork.EventBus.RabbitMq;

public interface IPropertiesProvider
{
    IBasicProperties GetProperties(IModel channel);
}