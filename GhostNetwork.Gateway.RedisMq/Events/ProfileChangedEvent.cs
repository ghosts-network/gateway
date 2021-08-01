using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.RedisMq.Events
{
    public class ProfileChangedEvent : EventBase
    {
        public User UpdatedUser { get; init; }
    }
}
