using MediatR;
using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Infrastructures;
using SampleProject.Domain.Interfaces.Application;

namespace SampleProject.Domain.Applications
{
    public class OrderApplication : BaseApplication, IOrderApplication
    {
        private readonly IMediator _mediator;

        public OrderApplication(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ApiResult<CreateResponse>> Create(CreateRequest request)
        {
            await _mediator.Publish(new OrderCreatedEvent { Id = Guid.Parse("90C4D81A-4C56-4880-89DE-FBAB1E5DA5C1") });
            //var testEventResponse = await _mediator.Send(new OrderCreatedCommand { });

            return null;


            var @event = MapperProvider.Map<CreateRequest>(request);

            var eventResponse = await _mediator.Send(@event);

            return HandleSuccess<CreateResponse>(MapperProvider.Map<CreateResponse>(eventResponse));
        }
    }
}