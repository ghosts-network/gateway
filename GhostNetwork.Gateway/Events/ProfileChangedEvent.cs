using GhostNetwork.Gateway.Users;

namespace GhostNetwork.Gateway.Events
{
    public class ProfileChangedEvent : BaseEvent
    {
        public User UpdatedUser { get; }

        public ProfileChangedEvent(string userTrigeredBy, User updatedUser)
        {
            TriggeredBy = userTrigeredBy;
            UpdatedUser = updatedUser;
        }
    }
}
