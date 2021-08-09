using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace GhostNetwork.Gateway.RedisMq
{
    public class RedisHandlerHostedService : IHostedService
    {
        private const int Timeout = 5000;

        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurationOptions redisConfiguration;
        private readonly string connectionString;
        private ConnectionMultiplexer conn;

        private Func<IServiceProvider, IEventWorker>[] args;

        public RedisHandlerHostedService(IServiceProvider serviceProvider, ConfigurationOptions redisConfiguration, params Func<IServiceProvider, IEventWorker>[] args)
        {
            this.serviceProvider = serviceProvider;
            this.redisConfiguration = redisConfiguration;
            this.args = args;
        }

        public RedisHandlerHostedService(IServiceProvider serviceProvider, string connectionString, params Func<IServiceProvider, IEventWorker>[] args)
        {
            this.serviceProvider = serviceProvider;
            this.connectionString = connectionString;
            this.args = args;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (redisConfiguration != null)
                {
                    conn = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
                }
                else
                {
                    conn = await ConnectionMultiplexer.ConnectAsync(connectionString);
                }
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            RunSubsribers(args.Select(arg => arg.Invoke(serviceProvider)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await conn.CloseAsync();
            conn.Dispose();
        }

        private void RunSubsribers(IEnumerable<IEventWorker> workers)
        {
            foreach (var worker in workers)
            {
                worker.Subscribe();
            }
        }
    }
}
