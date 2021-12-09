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
                TryConnect();
            }

            connection.ConnectionShutdown += OnConnectionLost;

            return connection;
        }

        /// <summary>
        /// Close connection to RabbitMq.
        /// </summary>
        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        private void OnConnectionLost(object sender, ShutdownEventArgs args)
        {
            lock (connectionLock)
            {
                Dispose();
                connection = null;

                TryConnect();
            }
        }

        private void TryConnect()
        {
            Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetryForever(retryAttempt =>
                    TimeSpan.FromMinutes(retryAttempt))
                .Execute(() =>
                {
                    connection ??= connectionFactory.CreateConnection();
                });
        }
    }
}