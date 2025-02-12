using MediatR;
using Microsoft.Extensions.Logging;
using SampleProject.Domain.Exceptions;
using System.Reflection;

namespace SampleProject.Domain.Applications.Adapter
{
    /// <summary>
    /// 歡樂鎖例外 Adapter
    /// </summary>
    public class OptimisticLockExceptionRertyAdapterHandler : INotificationPublisher
    {
        /// <summary>
        /// 歡樂鎖例外重試次數
        /// </summary>
        private const int OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES = 3;

        private readonly ILogger<OptimisticLockExceptionRertyAdapterHandler> _logger;

        public OptimisticLockExceptionRertyAdapterHandler(ILogger<OptimisticLockExceptionRertyAdapterHandler> logger)
        {
            _logger = logger;
        }

        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            foreach (var handler in handlerExecutors)
            {
                try
                {
                    var method = handler.HandlerInstance.GetType().GetMethod("Handle");
                    var hasRetryAttribute = method is not null &&
                                            method.GetCustomAttribute<RetryEventAttribute>() is not null;

                    if (hasRetryAttribute)
                    {
                        // 進行 retry，3 次後仍失敗則拋出例外
                        for (int i = 0; i < OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES; i++)
                        {
                            try
                            {
                                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);

                                break; // 成功則結束重試
                            }
                            catch (OptimisticLockException) when (i < OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES - 1)
                            {
                                //if (i == OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES - 1)
                                //{
                                //    throw; // 最後一次失敗則拋出
                                //}
                            }
                            catch (Exception)
                            {
                                // 不是 OptimisticLockException 的例外，直接拋出 (可能是自訂例外或者其他問題，不需要重複執行 event)
                                throw;
                            }
                        }
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