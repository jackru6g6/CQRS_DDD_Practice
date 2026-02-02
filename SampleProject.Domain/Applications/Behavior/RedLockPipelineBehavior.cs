using Castle.Core.Resource;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedLockNet;
using SampleProject.Domain.Interfaces.Behavior;
using System.Text.Json.Serialization;

namespace SampleProject.Domain.Applications.Behavior
{
    /// <summary>
    /// redlock PipelineBehavior
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RedLockPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<RedLockPipelineBehavior<TRequest, TResponse>> _logger;
        private readonly IDistributedLockFactory _redlockFactory;

        private static readonly TimeSpan _expiryTime = TimeSpan.FromSeconds(30.0);
        private static readonly TimeSpan _waitTime = TimeSpan.FromSeconds(20.0);
        private static readonly TimeSpan _retryTime = TimeSpan.FromMilliseconds(100.0);

        public RedLockPipelineBehavior(ILogger<RedLockPipelineBehavior<TRequest, TResponse>> logger,
                                       IDistributedLockFactory redlockFactory)
        {
            _redlockFactory = redlockFactory;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 只有當命令實作 IRequireDistributedLock 介面時，才應用鎖定邏輯
            if (request is IRequireRedLock lockableRequest)
            {
                using (var @lock = _redlockFactory.CreateLock(resource: lockableRequest.LockKey,
                                                              expiryTime: _expiryTime,
                                                              waitTime: _waitTime,
                                                              retryTime: _retryTime,
                                                              cancellationToken: cancellationToken))
                {
                    if (@lock.IsAcquired)
                    {
                        return await next();
                    }
                }

                // 同樣 key 互鎖，等待一段時間後仍無法取得鎖定，視為失敗
                throw new Exception($"Failed to acquire RedLock. Key: {lockableRequest.LockKey}, Command: {JsonConvert.SerializeObject(request)}.");
            }
            else
            {
                return await next();
            }
        }
    }
}
