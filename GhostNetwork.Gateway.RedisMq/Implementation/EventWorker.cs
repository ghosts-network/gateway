using GhostNetwork.Gateway.RedisMq.Events;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

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

        public async Task Subscribe<TEvent>(RedisKey key) where TEvent : EventBase, new()
        {
            while (true)
            {
                try
                {
                    var message = await db.ListLeftPopAsync(key);

                    if (message.HasValue)
                    {
                        foreach (var handler in CreateHandlers<TEvent>())
                        {
                            await Task.Run(() => handler.Handle(JsonSerializer.Deserialize<TEvent>(message)));
                        }
                    }
                    else Thread.Sleep(500);
                }
                catch (RedisConnectionException) 
                {  
                    Thread.Sleep(5000);
                }
            }
        }

        private IEnumerable<IEventHandler<TEvent>> CreateHandlers<TEvent>() where TEvent : EventBase, new()
        {
            var inheritingTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEventHandler<TEvent>).IsAssignableFrom(t));

            var typeOfHandlers = inheritingTypes.Where(types => 
                types.GetTypeInfo().ImplementedInterfaces
                    .Any(ii => ii.IsGenericType && ii.GetTypeInfo().GenericTypeArguments.Any(arg => arg.FullName == typeof(TEvent).FullName))
            );

            if (!typeOfHandlers.Any())
                throw new ArgumentException("Input type is not declared as inheritor of IEventHandler<Event>");

            var handlers = new List<IEventHandler<TEvent>>();
            
            foreach (var type in typeOfHandlers)
            {
                handlers.Add(serviceProvider.GetService(type) as IEventHandler<TEvent>);
            }

            return handlers;
        }
    }
}
