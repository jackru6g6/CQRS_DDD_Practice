using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SampleProject.Domain.Interfaces.Domain.Service;
using SampleProject.Domain.Interfaces.Infrastructure;
using System.Text;
using System.Text.Json;

namespace SampleProject.Domain.Domains.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannel _channel;

        public RabbitMQService(IServiceProvider serviceProvider, IRabbitMQConnection connection)
        {
            _serviceProvider = serviceProvider;
            _channel = connection.CreateChannel().Result;
        }

        /// <summary>
        /// 事件發送
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="integrationEvent"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task SendEventAsync<TEvent>(TEvent integrationEvent, string? queueName = null) where TEvent : INotification
        {
            queueName ??= typeof(TEvent).Name;

            await _channel.QueueDeclareAsync(queue: queueName,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

            var message = JsonSerializer.Serialize(integrationEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true // 設置消息持久化
            };

            await _channel.BasicPublishAsync(exchange: "",
                                             routingKey: queueName,
                                             basicProperties: properties,
                                             body: body,
                                             mandatory: false);
        }

        /// <summary>
        /// 事件聆聽
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerDelegate"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task StartEventListeningAsync<TEvent>(Func<IServiceProvider, INotificationHandler<TEvent>> eventHandlerDelegate, string? queueName = null, CancellationToken cancellationToken = default) where TEvent : INotification
        {
            queueName ??= typeof(TEvent).Name;

            await _channel.QueueDeclareAsync(queue: queueName,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    string? message = Encoding.UTF8.GetString(body);

                    var integrationEvent = JsonSerializer.Deserialize<TEvent>(message);

                    if (integrationEvent is not null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var eventHandler = eventHandlerDelegate(scope.ServiceProvider);
                            await eventHandler.Handle(integrationEvent, cancellationToken);
                        }
                    }

                    // 手動確認消息已被處理，配合 BasicConsumeAsync(autoAck:false)
                    //await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    // TODO：log 紀錄
                }
            };

            // autoAck true: 代表 RabbitMQ 會自動確認消息已被處理
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }

        /*
        /// <summary>
        /// 發送 MQ
        /// </summary>
        /// <remarks>
        /// 範例，單次使用
        /// </remarks>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="mandatory"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendAsync(string exchange, string routingKey, object message, bool mandatory = false, CancellationToken cancellationToken = default)
        {
            try
            {
                using var channel = await _connection.CreateChannel();
                var mesjson = JsonSerializer.Serialize(message);
                //Console.WriteLine($"發送訊息: {mesjson}");
                var body = Encoding.UTF8.GetBytes(mesjson);
                var properties = new RabbitMQ.Client.BasicProperties
                {
                    Persistent = true // 設置消息持久化
                };

                await channel.BasicPublishAsync(exchange: exchange,
                                                routingKey: routingKey,
                                                basicProperties: properties,
                                                body: body,
                                                mandatory: mandatory);
            }
            catch (OperationCanceledException ex)
            {
                //Console.WriteLine($"Operation was canceled: {ex.Message}");
                //throw; // Re-throw if you want to propagate the cancellation
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"An error occurred: {ex.Message}");
                //throw; // Re-throw if you want to propagate the error
            }
        }

        /// <summary>
        /// 接收 MQ
        /// </summary>
        /// <remarks>
        /// 範例，單次使用
        /// </remarks>
        /// <param name="queueName"></param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReceiveAsync(string queueName, Func<IChannel, byte[], Task> callback, CancellationToken cancellationToken = default)
        {
            var channel = await _connection.CreateChannel();
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                try
                {
                    // 直接傳遞 model 和 body 給 callback，不需要反序列化
                    await callback(channel, body);
                }
                finally
                {
                    // 手動確認消息已被處理
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

            // 阻止方法立即返回，直到取消令牌被觸發
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        */
    }
}
