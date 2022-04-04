﻿using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text.Json;

namespace GhostNetwork.EventBus.AzureServiceBus
{
    public class AzureServiceEventBus : IEventBus, IAsyncDisposable
    {
        private readonly IHandlerProvider handlerProvider;
        private readonly IMessageProvider messageProvider;
        private readonly INameProvider nameProvider;

        private readonly ServiceBusClient serviceBusClient;
        private readonly ServiceBusAdministrationClient subscriptionManager;

        private IList<ServiceBusProcessor> _processorList;

        public AzureServiceEventBus(string connectionString, IHandlerProvider handlerProvider, IMessageProvider? messageProvider = null, INameProvider? nameProvider = null)
        {
            serviceBusClient = new ServiceBusClient(connectionString);
            subscriptionManager = new ServiceBusAdministrationClient(connectionString);
            _processorList = new List<ServiceBusProcessor>();

            this.handlerProvider = handlerProvider;

            this.nameProvider = nameProvider ?? new DefaultNameProvider();
            this.messageProvider = messageProvider ?? new JsonMessageProvider();
        }

        public async  Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            var topicName = nameProvider.GetTopicName<TEvent>();

            await using var sender = serviceBusClient.CreateSender(topicName);
            var body = messageProvider.GetMessage(@event);

            await sender.SendMessageAsync(new ServiceBusMessage(body));
            await sender.CloseAsync();
        }

        public async void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            string topicName = nameProvider.GetTopicName<TEvent>();
            string subscriptionName = nameProvider.GetSubscriptionName<THandler, TEvent>();

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
                var message = messageProvider.GetEvent(args.Message.Body.ToArray(), typeof(TEvent)) as TEvent;
                var handler = handlerProvider.GetRequiredService<TEvent>(typeof(THandler));
                
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
            var topicName = nameProvider.GetTopicName<TEvent>();
            var subscriptionName = nameProvider.GetSubscriptionName<THandler, TEvent>();

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