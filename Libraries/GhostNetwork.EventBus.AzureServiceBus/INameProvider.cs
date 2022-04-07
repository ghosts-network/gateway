namespace GhostNetwork.EventBus.AzureServiceBus
{
    public interface INameProvider
    {
        string GetTopicName<TEvent>() where TEvent : Event;
        string GetSubscriptionName<THandler, TEvent>() 
            where TEvent : Event
            where THandler : IEventHandler<TEvent>;
    }
}