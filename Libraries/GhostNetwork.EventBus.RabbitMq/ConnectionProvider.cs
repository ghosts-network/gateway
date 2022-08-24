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
            if (connection == null)
            {
                lock (connectionLock)
                {
                    connection = connectionFactory.CreateConnection();
                }
            }

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
    }
}