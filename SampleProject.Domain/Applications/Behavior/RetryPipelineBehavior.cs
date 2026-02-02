using MediatR;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Interceptors;
using System.Reflection;

namespace SampleProject.Domain.Applications.Behavior
{
    public class RetryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request,
                                      RequestHandlerDelegate<TResponse> next, // next 代理了 IRequestHandler 的 Handle 呼叫
                                      CancellationToken cancellationToken)
        {

            var a = next.GetMethodInfo();
            var aa = a.GetCustomAttribute<RetryEventAttribute>();

            // 有可能有多個方法，多判斷傳入參數是否符合
            var method = next.GetMethodInfo().GetType().GetMethods()
                             .Where(m => m.Name == "Handle" &&
                                            m.GetParameters().Any(p => p.ParameterType == request.GetType()))
                                .FirstOrDefault();

            var hasRetryAttribute = method is not null &&
                                    method.GetCustomAttribute<RetryEventAttribute>() is not null;



            // 將 IRequestHandler 的執行封裝在重試工具中
            return await RetryUtility.ExecuteWithRetry(async () => await next());


            // 判斷 TRequest 是否實現了 IRetryableRequest 介面
            //if (request is IRetryableRequest retryableRequest)
            //{
            //    // 如果實現了，則執行重試邏輯
            //    // 您甚至可以從 retryableRequest.MaxAttempts 獲取重試次數
            //    return await RetryUtility.ExecuteWithRetry(async () => await next());
            //}
            //else
            //{
            //    // 否則，直接執行
            //    return await next();
            //}
        }
    }
}
