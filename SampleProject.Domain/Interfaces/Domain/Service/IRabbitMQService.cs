using MediatR;

namespace SampleProject.Domain.Interfaces.Domain.Service
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// 事件發送
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="integrationEvent"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task SendEventAsync<TEvent>(TEvent integrationEvent, string? queueName = null) where TEvent : INotification;

        /// <summary>
        /// 事件聆聽
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerDelegate"></param>
        /// <param name="queueName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartEventListeningAsync<TEvent>(Func<IServiceProvider, INotificationHandler<TEvent>> eventHandlerDelegate, string? queueName = null, CancellationToken cancellationToken = default) where TEvent : INotification;
    }
}
