using GhostNetwork.Gateway.RedisMq.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq.Handlers
{
    public class NadeEventHandler : IEventHandler<NadoEvent>
    {
        public Task Handle(EventBase value)
        {
            // do some work with model

            return Task.CompletedTask;
        }
    }

    public class NadeEventHandlerTwo: IEventHandler<NadoEvent>
    {
        public Task Handle(EventBase value)
        {
            // do some work with model

            return Task.CompletedTask;
        }
    }
}
