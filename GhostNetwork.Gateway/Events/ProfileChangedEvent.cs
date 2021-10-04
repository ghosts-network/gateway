using GhostEventBus.Events;
using GhostNetwork.Gateway;

public class ProfileChangedEvent : EventBase
{
    public UserInfo UpdatedUser { get; init; }
}