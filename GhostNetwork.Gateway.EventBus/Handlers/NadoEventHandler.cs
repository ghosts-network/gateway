using GhostNetwork.Gateway.EventBus.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.EventBus.RedisMq
{
    public class NadeEventHandler : IEventHandler<NadoEvent>
    {
        public Task Handle(EventBase value)
        {
            // do some work with model

            return Task.CompletedTask;
        }

        public Task Handle(NadoEvent value)
        {
            throw new System.NotImplementedException();
        }
    }

    public class NadeEventHandlerTwo: IEventHandler<NadoEvent>
    {
        public Task Handle(EventBase value)
        {
            // do some work with model

            return Task.CompletedTask;
        }

        public Task Handle(NadoEvent value)
        {
            throw new System.NotImplementedException();
        }
    }
}
