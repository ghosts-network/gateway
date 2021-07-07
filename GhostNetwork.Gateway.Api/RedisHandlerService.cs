using GhostNetwork.Gateway.Events;
using GhostNetwork.Gateway.RedisMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Api
{
    class RedisHandlerService : IHostedService
    {
        private ConnectionMultiplexer _conn;
        private readonly IConfiguration configuration;

        private const int _timeout = 5000;

        public RedisHandlerService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                ConfigurationOptions config = new ConfigurationOptions
                {
                    ConnectTimeout = _timeout,
                    ReconnectRetryPolicy = new LinearRetry(_timeout),
                    EndPoints =
                    {
                        { "127.0.0.1", 50002 }
                    }
                };

                _conn = await ConnectionMultiplexer.ConnectAsync(config);
            }
            catch (RedisTimeoutException)
            {
                throw;
            }

            RunSubsribers(new EventMessageHandler(_conn.GetDatabase()));
        }

        private void RunSubsribers(EventMessageHandler handler)
        {
            Task.Run(() => handler.Subscribe<ProfileChangedEvent>(nameof(ProfileChangedEvent)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _conn.CloseAsync();
            _conn.Dispose();
        }
    }
}
