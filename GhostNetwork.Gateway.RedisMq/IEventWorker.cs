using System;
using GhostNetwork.Gateway.RedisMq.Events;

namespace GhostNetwork.Gateway.RedisMq
{
    public interface IEventWorker
    {
        void Subscribe<T>() where T : EventBase, new();
    }
}
