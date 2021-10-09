using System.Threading.Tasks;
using GhostEventBus;
using GhostEventBus.Events;

public class NullEventSender : IEventSender
{
    public Task PublishAsync(EventBase @event)
    {
        return Task.CompletedTask;
    }
} 