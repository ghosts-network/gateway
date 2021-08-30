using GhostEventBus.Events;
using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.Events
{
    public class ProfileChangedEvent : EventBase
    {
        public User UpdatedUser { get; init; }
    }
}