using GhostNetwork.Gateway.EventBus.Events;
using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.Events
{
    public class ProfileChangedEvent : EventBase
    {
        public User User { get; init; }
    }
}
