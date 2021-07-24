using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

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
                try
                {
                    var message = await db.ListLeftPopAsync(key);

                    if (message.HasValue)
                    {
                        await CreateHandler<TEvent>().Handle(JsonSerializer.Deserialize<TEvent>(message));
                    }
                    else Thread.Sleep(500);
                }
                catch (RedisConnectionException) 
                {  
                    Thread.Sleep(5000);
                }
            }
        }

        private IEventHandler<TEvent> CreateHandler<TEvent>() where TEvent : IEvent, new()
        {
            var inheritingTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEventHandler<TEvent>).IsAssignableFrom(t));

            var typeOfHandler = inheritingTypes.Where(types => 
                types.GetTypeInfo().ImplementedInterfaces
                    .Any(ii => ii.IsGenericType && ii.GetTypeInfo().GenericTypeArguments.Any(arg => arg.FullName == typeof(TEvent).FullName))
            ).First() ?? throw new ArgumentException("");

            return serviceProvider.GetService(typeOfHandler) as IEventHandler<TEvent>;
        }
    }
}
