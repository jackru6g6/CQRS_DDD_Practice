using RabbitMQ.Client;

namespace SampleProject.Domain.Interfaces.Infrastructure
{
    public interface IRabbitMQConnection : IDisposable
    {
        Task<IChannel> CreateChannel();
    }
}
