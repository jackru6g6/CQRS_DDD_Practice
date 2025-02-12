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

        protected async void SendEvent(IAggregateRoot aggregateRoot)
        {
            await _mediator.DispatchDomainEventsAsync(aggregateRoot);
        }
    }
}
