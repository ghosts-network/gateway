using System;
using RabbitMQ.Client;

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
                // TODO: Handle BrokerUnreachableException
                connection ??= connectionFactory.CreateConnection();
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