using MediatR;
using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Applications.Adapter
{
    /// <summary>
    /// 歡樂鎖例外Adapter
    /// </summary>
    public class OptimisticLockExceptionRertyAdapterHandler : INotificationPublisher
    {
        /// <summary>
        /// 歡樂鎖例外重試次數
        /// </summary>
        private const int OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES = 3;

        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            foreach (var handler in handlerExecutors)
            {
                // 進行retry，3次後仍失敗則拋出例外，必須確保各個handler的執行只對單一domain，這樣才能確保retry的正確性
                for (int i = 0; i < OPTIMISTIC_LOCK_EXCEPTION_RETRY_TIMES; i++)
                {
                    try
                    {
                        await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);

                        break;
                    }
                    catch (OptimisticLockException)
                    {
                        if (i == 2)
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }
}