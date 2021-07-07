using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public class EventSender : IEventSender
    {
        private readonly IDatabase db;

        public EventSender(IDatabase db)
        {
            this.db = db;
        }

        public async Task PublishAsync(IEvent @event)
        {
            switch (@event)
            {
                case ProfileChangedEvent e:
                    await db.ListRightPushAsync(nameof(ProfileChangedEvent), JsonSerializer.Serialize(e));
                    break;
            }
        }
    }
}
