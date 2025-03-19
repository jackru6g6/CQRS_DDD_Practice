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
        /// </remarks>
        /// <param name="aggregateRoot"></param>
        protected async void SendEvent(IAggregateRoot aggregateRoot)
        {
            await _mediator.DispatchDomainEventsAsync(aggregateRoot);
        }
    }
}
