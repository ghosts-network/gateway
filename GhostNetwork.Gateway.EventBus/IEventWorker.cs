using System;
using GhostNetwork.Gateway.EventBus.Events;

namespace GhostNetwork.Gateway.EventBus
{
    public interface IEventWorker
    {
        void Subscribe<TEvent>() where TEvent : EventBase, new();
        
        void Subscribe(string key, Type type);
    }
}
