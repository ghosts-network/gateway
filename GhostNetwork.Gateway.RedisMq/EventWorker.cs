using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace GhostNetwork.Gateway.RedisMq
{
    public class EventWorker : IEventWorker
    {
        private readonly IDatabase db;
        private readonly IServiceProvider serviceProvider;

        public EventWorker(IDatabase db, IServiceProvider serviceProvider)
        {
            this.db = db;
            this.serviceProvider = serviceProvider;
        }

        public async Task Subscribe<TEvent>(RedisKey key) where TEvent : IEvent, new()
        {
            while (true)
            {
                var message = await db.ListLeftPopAsync(key);

                if (message.HasValue)
                {
                    await CreateHandler<TEvent>().Handle(JsonSerializer.Deserialize<TEvent>(message));
                }
                else Thread.Sleep(500);
            }
        }

        private IEventHandler<TEvent> CreateHandler<TEvent>() where TEvent : IEvent, new()
        {
            switch (new TEvent())
            {
                case ProfileChangedEvent:
                    return serviceProvider.GetService<ProfileChangedEventHandler>() as IEventHandler<TEvent>;

                default:
                    throw new ArgumentException("Something went wrong");
            }
        }
    }
}
