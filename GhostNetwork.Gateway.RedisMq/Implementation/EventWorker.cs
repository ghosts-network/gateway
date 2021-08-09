using GhostNetwork.Gateway.RedisMq.Events;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using GhostNetwork.Gateway.RedisMq.Extensions;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    internal class EventWorker<TEvent> : IEventWorker<TEvent> where TEvent : EventBase, new() 
    {
        private readonly IDatabase db;
        private readonly IServiceProvider serviceProvider;

        public EventWorker(IDatabase db, IServiceProvider serviceProvider)
        {
            this.db = db;
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            Task.Run(async () => {
                while (true)
                {
                    try
                    {
                        var message = await db.ListLeftPopAsync(typeof(TEvent).Name);

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
            });
        }
    }
}
