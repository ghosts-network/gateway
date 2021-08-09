using GhostNetwork.Gateway.RedisMq.Events;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventWorker 
    {
        void Subscribe();
    }
    public interface IEventWorker<out TEvent> : IEventWorker
    {
        void Subscribe();
    }
}
