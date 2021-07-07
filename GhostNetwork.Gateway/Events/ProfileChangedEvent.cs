using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.Events
{
    public interface IEvent
    {
        string TriggeredBy { get; init; }
    }

    public class ProfileChangedEvent : IEvent
    {
        public User UpdatedUser { get; init; }
        public string TriggeredBy { get; init; }
    }
}
