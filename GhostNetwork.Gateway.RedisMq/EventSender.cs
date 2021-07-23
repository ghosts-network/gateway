using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    public class EventSender : IEventSender
    {
        private readonly IDatabase db;
        private bool IsAvailable { get; set; }

        public EventSender(IDatabase db)
        {
            this.db = db;
            
            if (this.db != null)
            {
                IsAvailable = true;
            }
        }

        public async Task PublishAsync(IEvent @event)
        {
            if (!(await CheckAndRestoreConnection()))
            {
                return;
            }

            try
            {
                switch (@event)
                {
                    case ProfileChangedEvent e:
                        await db.ListRightPushAsync(nameof(ProfileChangedEvent), JsonSerializer.Serialize(e));
                        break;
                }
            } 
            catch (RedisConnectionException)
            {
                IsAvailable = false;
            }
        }

        private async Task<bool> CheckAndRestoreConnection()
        {
            if (!IsAvailable)
            {
                try
                {
                    await db.PingAsync();
                }
                catch (RedisConnectionException)
                {
                    return false;
                }

                IsAvailable = true;
            }

            return true;
        }
    }
}
