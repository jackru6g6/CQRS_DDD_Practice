using MediatR;
using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;
using SampleProject.Domain.Infrastructures;

namespace SampleProject.Domain.Applications
{
    public class OrderApplication : BaseApplication
    {
        private readonly IMediator _mediator;

        public OrderApplication(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ApiResult<CreateResponse>> Create(CreateRequest request)
        {
            var @event = MapperProvider.Map<CreateRequest>(request);

            var eventResponse = await _mediator.Send(@event);

            return HandleSuccess<CreateResponse>(MapperProvider.Map<CreateResponse>(eventResponse));
        }

    }
}