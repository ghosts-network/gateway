using GhostNetwork.Gateway.RedisMq.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq.Handlers
{
    public class ProfileChangedEventHandler : IEventHandler<ProfileChangedEvent>
    {
        public Task Handle(ProfileChangedEvent value)
        {
            // do some work with model

            return Task.CompletedTask;
        }
    }
}
