using GhostNetwork.Gateway.EventBus.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.EventBus.RedisMq
{
    public class ProfileChangedEventHandler : IEventHandler<ProfileChangedEvent>
    {
        public Task Handle(EventBase value)
        {
            // do some work with model

            return Task.CompletedTask;
        }

        public Task Handle(ProfileChangedEvent value)
        {
            throw new System.NotImplementedException();
        }
    }
}
