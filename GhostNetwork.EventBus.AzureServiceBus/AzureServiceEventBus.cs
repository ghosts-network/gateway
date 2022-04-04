using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text.Json;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public class AzureServiceEventBus : IEventBus, IAsyncDisposable
    {
        private readonly IHandlerProvider _handlerProvider;
        private readonly IMessageProvider _messageProvider;
        private readonly INameProvider _nameProvider;

        private readonly ServiceBusClient serviceBusClient;
        private readonly ServiceBusAdministrationClient subscriptionManager;

        private IList<ServiceBusProcessor> _processorList;

        public AzureServiceEventBus(string connectionString, IHandlerProvider handlerProvider, IMessageProvider? messageProvider = null, INameProvider? nameProvider = null)
        {
            serviceBusClient = new ServiceBusClient(connectionString);
            subscriptionManager = new ServiceBusAdministrationClient(connectionString);
            _processorList = new List<ServiceBusProcessor>();

            _handlerProvider = handlerProvider;

            _nameProvider = nameProvider ?? new DefaultNameProvider();
            _messageProvider = messageProvider ?? new JsonMessageProvider();
        }

        public async  Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            var topicName = _nameProvider.GetTopicName<TEvent>();

            await using var sender = serviceBusClient.CreateSender(topicName);
            var body = JsonSerializer.Serialize(@event);

            await sender.SendMessageAsync(new ServiceBusMessage(body));
            await sender.CloseAsync();
        }

        public async void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            string topicName = _nameProvider.GetTopicName<TEvent>();
            string subscriptionName = _nameProvider.GetSubscriptionName<THandler, TEvent>();

            if (!await subscriptionManager.TopicExistsAsync(topicName))
            {
                await subscriptionManager.CreateTopicAsync(topicName);
            }

            if (!await subscriptionManager.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await subscriptionManager.CreateSubscriptionAsync(topicName, subscriptionName);
            }

            var processor = serviceBusClient.CreateProcessor(topicName, subscriptionName);
            _processorList.Add(processor);

            processor.ProcessMessageAsync += async args =>
            {
                var message = _messageProvider.GetEvent(args.Message.Body.ToArray(), typeof(TEvent)) as TEvent;
                var handler = _handlerProvider.GetRequiredService<TEvent>(typeof(THandler));
                
                await handler.ProcessAsync(message!);
                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                throw args.Exception;
            };

            await processor.StartProcessingAsync();
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            var topicName = _nameProvider.GetTopicName<TEvent>();
            var subscriptionName = _nameProvider.GetSubscriptionName<THandler, TEvent>();

            subscriptionManager.DeleteSubscriptionAsync(topicName, subscriptionName).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await serviceBusClient.DisposeAsync();
            foreach (var proccesor in _processorList)
            {
                await proccesor.DisposeAsync();
            }
        }
    }
}