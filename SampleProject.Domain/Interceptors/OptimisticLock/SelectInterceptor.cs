using Castle.DynamicProxy;

namespace SampleProject.Domain.Filters.OptimisticLock
{
    //[AttributeUsage(AttributeTargets.Method)]
    public class SelectInterceptor : IInterceptor
    {
        public SelectInterceptor()
        {
                
        }

        public void Intercept(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
