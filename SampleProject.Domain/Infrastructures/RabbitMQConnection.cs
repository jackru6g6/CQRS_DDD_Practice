using RabbitMQ.Client;
using SampleProject.Domain.Interfaces.Infrastructure;

namespace SampleProject.Domain.Infrastructures
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnection _connection;
        private bool _isDisposed;

        private RabbitMQConnection(IConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 建立 RabbitMQConnection 的非同步工廠方法，避免建構函式中使用 .Result 造成 Deadlock。
        /// </summary>
        public static async Task<RabbitMQConnection> CreateAsync(ConnectionFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            var connection = await factory.CreateConnectionAsync();
            return new RabbitMQConnection(connection);
        }

        public async Task<IChannel> CreateChannel()
        {
            EnsureNotDisposed();
            return await _connection.CreateChannelAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _connection?.Dispose();
            }

            _isDisposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
