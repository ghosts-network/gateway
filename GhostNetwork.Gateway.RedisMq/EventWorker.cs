using GhostNetwork.Gateway.Events;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.RedisMq
{
    class EventWorker : IEventWorker
    {
        public Task Handle<T>(RedisValue value) where T : BaseEvent
        {
            var model = JsonSerializer.Deserialize<T>(
                            BitConverter.ToString(value.Box() as byte[]));

            // do some work with model

            return Task.CompletedTask;
        }
    }
}
