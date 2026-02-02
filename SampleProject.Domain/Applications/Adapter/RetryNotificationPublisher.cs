using MediatR;
using Microsoft.Extensions.Logging;
using SampleProject.Domain.Interceptors;
using System.Reflection;

namespace SampleProject.Domain.Applications.Adapter
{
    public class RetryNotificationPublisher : INotificationPublisher
    {
        private readonly ILogger<RetryNotificationPublisher> _logger;

        public RetryNotificationPublisher(ILogger<RetryNotificationPublisher> logger)
        {
            _logger = logger;
        }

        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            foreach (var handler in handlerExecutors)
            {
                try
                {
                    // 有可能有多個方法，多判斷傳入參數是否符合
                    var method = handler.HandlerInstance.GetType().GetMethods()
                                        .Where(m => m.Name == "Handle" &&
                                                    m.GetParameters().Any(p => p.ParameterType == notification.GetType()))
                                        .FirstOrDefault();

                    var hasRetryAttribute = method is not null &&
                                            method.GetCustomAttribute<RetryEventAttribute>() is not null;
                    if (hasRetryAttribute)
                    {
                        // 將每個 handler 的 Handle 方法視為一個獨立的 operation
                        await RetryUtility.ExecuteWithRetryTask(async () =>
                        {
                            await handler.HandlerCallback(notification, cancellationToken)
                                         .ConfigureAwait(false);
                        });
                    }
                    else
                    {
                        // 如果沒有 RetryEventAttribute，則不進行重試，直接執行一次
                        await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "執行 event 發生例外");
                }
            }
        }
    }
}
