using GhostNetwork.Gateway.RedisMq.Events;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using GhostNetwork.Gateway.RedisMq.Extensions;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    internal class EventWorker : IEventWorker
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
                        foreach (var handler in serviceProvider.GetHandlers<TEvent>())
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

        public async Task Subscribe(RedisKey key, Type type) 
        {
            while (true)
            {
                try
                {
                    var message = await db.ListLeftPopAsync(key);

                    if (message.HasValue)
                    {
                        foreach (var handler in serviceProvider.GetHandlers(type))
                        {
                            await Task.Run(() => handler.Handle(JsonSerializer.Deserialize(message, type) as EventBase));
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
    }
}
