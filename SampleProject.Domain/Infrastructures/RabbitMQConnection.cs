using RabbitMQ.Client;
using SampleProject.Domain.Interfaces.Infrastructure;

namespace SampleProject.Domain.Infrastructures
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private bool _isDisposed;

        public RabbitMQConnection(ConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _connection = factory.CreateConnectionAsync().Result;
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

            /*
            if (disposing)
            {
                try
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                catch (IOException ex)
                {
                    // 可以記錄日誌或處理連接關閉時的異常
                    Console.WriteLine($"關閉 RabbitMQ 連接時發生錯誤: {ex.Message}");
                }
            }
            */

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _connection?.Dispose();

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
