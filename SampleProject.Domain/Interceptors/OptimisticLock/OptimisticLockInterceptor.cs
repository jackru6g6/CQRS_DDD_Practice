using Castle.DynamicProxy;
using SampleProject.Domain.Interceptors.OptimisticLock.Attribute;

namespace SampleProject.Domain.Filters.OptimisticLock
{
    public class OptimisticLockInterceptor : IInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public OptimisticLockInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheableAttribute = invocation.MethodInvocationTarget
                                               .GetCustomAttributes(typeof(SelectAttribute), true)
                                               .FirstOrDefault() as SelectAttribute;

            if (cacheableAttribute != null)
            {
                //var cacheService = _serviceProvider.GetRequiredService<ICacheService>();
                //var cacheKey = cacheableAttribute.CacheKey;

                //var cachedData = cacheService.GetCachedData<object>(cacheKey);

                //if (cachedData != null)
                //{
                //    invocation.ReturnValue = cachedData;

                //    return;
                //}

                invocation.Proceed();

                var a = invocation.ReturnValue;

                //cacheService.CacheData(cacheKey, invocation.ReturnValue);
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
