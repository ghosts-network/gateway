using System;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Events;
using GhostNetwork.Gateway.RedisMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace GhostNetwork.Gateway.Api
{
    class RedisHandlerHostedService : IHostedService
    {
        private const int Timeout = 5000;

        private readonly IServiceProvider serviceProvider;
        private ConnectionMultiplexer conn;

        public RedisHandlerHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                ConfigurationOptions config = new ConfigurationOptions
                {
                    ConnectTimeout = Timeout,
                    ReconnectRetryPolicy = new LinearRetry(Timeout),
                    EndPoints =
                    {
                        { "127.0.0.1", 50002 }
                    }
                };

                conn = await ConnectionMultiplexer.ConnectAsync(config);
            }
            catch (RedisTimeoutException)
            {
                throw;
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
