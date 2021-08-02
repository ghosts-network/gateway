using System;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.RedisMq.Events;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace GhostNetwork.Gateway.RedisMq
{
    public class RedisHandlerHostedService : IHostedService
    {
        private const int Timeout = 5000;

        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurationOptions redisConfiguration;
        private ConnectionMultiplexer conn;

        public RedisHandlerHostedService(IServiceProvider serviceProvider, ConfigurationOptions redisConfiguration)
        {
            this.serviceProvider = serviceProvider;
            this.redisConfiguration = redisConfiguration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                conn = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            RunSubsribers();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await conn.CloseAsync();
            conn.Dispose();
        }

        private void RunSubsribers()
        {
            Task.Run(() => new EventWorker(conn.GetDatabase(), serviceProvider)
                .Subscribe<ProfileChangedEvent>(nameof(ProfileChangedEvent)));
        }
    }
}
