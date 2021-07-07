﻿using GhostNetwork.Gateway.Events;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
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