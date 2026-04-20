using Castle.DynamicProxy;
using MediatR;
using SampleProject.Domain.Extensions;
using SampleProject.Domain.Interceptors.OptimisticLock.Attribute;
using SampleProject.Domain.Interfaces.Domain;

namespace SampleProject.Domain.Interceptors.DomainEvent
{
    /// <summary>
    /// 自動發送 Domain Event 攔截器
    /// </summary>
    /// <remarks>
    /// 攔截 Add、Update、Delete 方法，執行完畢後自動呼叫 DispatchDomainEventsAsync。
    /// 方法第一個參數必須實作 IAggregateRoot。
    /// </remarks>
    public class DomainEventInterceptor : IInterceptor
    {
        private readonly IMediator _mediator;

        public DomainEventInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Intercept(IInvocation invocation)
        {
            var attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true);

            var isAdd    = attributes.OfType<AddAttribute>().Any();
            var isUpdate = attributes.OfType<UpdateAttribute>().Any();
            var isDelete = attributes.OfType<DeleteAttribute>().Any();

            // 先執行原始方法
            invocation.Proceed();

            // Add / Update / Delete 後自動發送 Domain Event
            if (isAdd || isUpdate || isDelete)
            {
                var aggregateRoot = invocation.Arguments
                    .OfType<IAggregateRoot>()
                    .FirstOrDefault();

                if (aggregateRoot is not null)
                {
                    _mediator.DispatchDomainEventsAsync(aggregateRoot).GetAwaiter().GetResult();
                }
            }
        }
    }
}
