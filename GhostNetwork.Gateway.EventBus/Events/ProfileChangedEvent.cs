using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.EventBus.Events
{
    public class ProfileChangedEvent : EventBase
    {
        public User UpdatedUser { get; init; }
    }
}
