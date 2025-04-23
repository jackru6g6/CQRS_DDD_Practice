using MediatR;
using SampleProject.Domain.Extensions;
using SampleProject.Domain.Interfaces.Domain;

namespace SampleProject.Domain.Repositories
{
    public class BaseRepository
    {
        private readonly IMediator _mediator;

        public BaseRepository(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 發送通知
        /// </summary>
        /// <remarks>
        /// 目前放這邊，但不應該放 Repository，應該獨立一個，發送通知不屬於 Repository 的職責。
        /// 可能可以設計一個AOP，執行完 Repository 的方法後，會自動發送通知。
        /// </remarks>
        /// <param name="aggregateRoot"></param>
        protected async void SendEvent(IAggregateRoot aggregateRoot)
        {
            await _mediator.DispatchDomainEventsAsync(aggregateRoot);
        }
    }
}
