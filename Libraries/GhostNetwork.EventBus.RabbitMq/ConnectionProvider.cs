using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace GhostNetwork.EventBus.RabbitMq
{
    internal class ConnectionProvider : IDisposable
    {
        private readonly ConnectionFactory connectionFactory;

        private readonly object connectionLock = new();

        private IConnection connection;

        public ConnectionProvider(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IConnection GetConnection()
        {
            lock (connectionLock)
            {
                Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetryForever(retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .Execute(() =>
                {
                    connection ??= connectionFactory.CreateConnection();
                });
            }

            return connection;
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}