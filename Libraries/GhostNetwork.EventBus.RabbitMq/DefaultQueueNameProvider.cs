namespace GhostNetwork.EventBus.RabbitMq
{
    public class DefaultQueueNameProvider : INameProvider
    {
        public string GetQueueName<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            return typeof(THandler).FullName!.ToLower();
        }

        public string GetExchangeName<TEvent>()
            where TEvent : Event
        {
            var name = typeof(TEvent).FullName!.ToLower();
            if (name.EndsWith("event"))
            {
                name = name[..^5];
            }

            return name;
        }
    }
}