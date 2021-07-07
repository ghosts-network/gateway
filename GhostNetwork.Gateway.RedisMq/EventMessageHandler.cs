using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public class EventMessageHandler : IEventMessageHandler
    {
        private readonly IDatabase db;

        public EventMessageHandler(IDatabase db)
        {
            this.db = db;
        }

        public async Task Subscribe<T>(RedisKey key) where T : BaseEvent
        {
            while (true)
            {
                var message = await db.ListLeftPopAsync(key);

                if (message.HasValue)
                {
                    await new EventWorker().Handle<T>(message);
                }
            }
        }
    }
}
