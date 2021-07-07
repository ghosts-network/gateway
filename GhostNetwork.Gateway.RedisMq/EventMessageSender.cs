using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public class EventMessageSender : IEventMessageSender
    {
        private readonly IDatabase db;

        public EventMessageSender(IDatabase db)
        {
            this.db = db;
        }

        public async Task PublishAsync<T>(BaseEvent @event) where T : BaseEvent
        {
            await db.ListRightPushAsync(nameof(T), JsonSerializer.Serialize(@event as T));
        }
    }
}
