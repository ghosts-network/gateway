using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GhostNetwork.EventBus.RabbitMq
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly ConnectionProvider connectionProvider;
        private readonly IMessageProvider messageProvider;
        private readonly IPropertiesProvider propertiesProvider;
        private readonly INameProvider nameProvider;
        private readonly IHandlerProvider handlerProvider;
        private readonly SubscriptionManager subscriptionManager;

        [Obsolete("Use constructor with IPropertiesProvider")]
        public RabbitMqEventBus(
            ConnectionFactory connectionFactory,
            IHandlerProvider handlerProvider,
            IMessageProvider messageProvider = null,
            INameProvider nameProvider = null)
        {
            connectionProvider = new ConnectionProvider(connectionFactory);
            subscriptionManager = new SubscriptionManager();
            this.handlerProvider = handlerProvider;
            this.messageProvider = messageProvider ?? new JsonMessageProvider();
            this.nameProvider = nameProvider ?? new DefaultQueueNameProvider();
            propertiesProvider = new EmptyPropertiesProvider();
        }

        public RabbitMqEventBus(
            ConnectionFactory connectionFactory,
            IHandlerProvider handlerProvider,
            IMessageProvider messageProvider = null,
            IPropertiesProvider propertiesProvider = null,
            INameProvider nameProvider = null)
        {
            connectionProvider = new ConnectionProvider(connectionFactory);
            subscriptionManager = new SubscriptionManager();
            this.handlerProvider = handlerProvider;
            this.messageProvider = messageProvider ?? new JsonMessageProvider();
            this.propertiesProvider = propertiesProvider ?? new EmptyPropertiesProvider();
            this.nameProvider = nameProvider ?? new DefaultQueueNameProvider();
        }

        public Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : Event
        {
            using var channel = connectionProvider.GetConnection().CreateModel();

            var exchangeName = nameProvider.GetExchangeName<TEvent>();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            var props = propertiesProvider.GetProperties(channel);
            if (string.IsNullOrEmpty(props.MessageId))
            {
                props.MessageId = Guid.NewGuid().ToString();
            }

            channel.BasicPublish(
                exchangeName,
                string.Empty,
                props,
                messageProvider.GetMessage(@event));

            channel.Close();
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>()
            where THandler : IEventHandler<TEvent>
            where TEvent : Event
        {
            var channel = connectionProvider.GetConnection().CreateModel();

            var exchangeName = nameProvider.GetExchangeName<TEvent>();
            var queueName = nameProvider.GetQueueName<TEvent, THandler>();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            channel.QueueDeclare(queueName);
            channel.QueueBind(queueName, exchangeName, string.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var message = messageProvider.GetEvent(ea.Body.ToArray(), typeof(TEvent)) as TEvent;
                var handler = handlerProvider.GetRequiredService<TEvent>(typeof(THandler));
                handler.ProcessAsync(message);
            };

            channel.BasicConsume(queueName, true, consumer);
            subscriptionManager.Subscribe<TEvent, THandler>(channel);
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            subscriptionManager.Unsubscribe<TEvent, THandler>();
        }

        public void Dispose()
        {
            connectionProvider?.Dispose();
        }
    }
}