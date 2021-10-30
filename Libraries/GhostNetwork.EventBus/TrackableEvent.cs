using System;

namespace GhostNetwork.EventBus
{
    public abstract record TrackableEvent : Event
    {
        protected TrackableEvent()
        {
            CreatedOn = DateTimeOffset.UtcNow;
            TrackerId = Guid.NewGuid();
        }

        public DateTimeOffset CreatedOn { get; }
        public Guid TrackerId { get; }
    }
}