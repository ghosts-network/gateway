using System;
using Microsoft.Extensions.DependencyInjection;

namespace GhostNetwork.EventBus.RabbitMq
{
    public class HandlerProvider : IHandlerProvider
    {
        private readonly IServiceProvider serviceProvider;

        public HandlerProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEventHandler<TEvent> GetRequiredService<TEvent>(Type type)
            where TEvent : Event
        {
            var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService(type) as IEventHandler<TEvent>;
        }
    }
}
