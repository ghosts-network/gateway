namespace GhostNetwork.EventBus.AzureServiceBus
{
    public class DefaultNameProvider : INameProvider
    {
        public string GetTopicName<TEvent>() where TEvent : Event
        {
            var name = typeof(TEvent).FullName!.ToLower();
            if (name.EndsWith("event"))
            {
                name = name[..^5];
            }

            return name;
        }

        public string GetSubscriptionName<THandler, TEvent>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            return typeof(THandler).FullName!.ToLower();
        }
    }
}