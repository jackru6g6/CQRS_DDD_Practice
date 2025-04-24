using MediatR;
using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Interfaces.Application;
using SampleProject.Domain.Interfaces.Domain.Service;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Applications
{
    public class OrderApplication : BaseApplication, IOrderApplication
    {
        private readonly IMediator _mediator;
        private readonly IOrderAggRepository _repo;

        // for test
        private readonly IRabbitMQService _rabbitMQService;

        public OrderApplication(IMediator mediator, IOrderAggRepository repo, IRabbitMQService rabbitMQService)
        {
            _mediator = mediator;
            _repo = repo;

            _rabbitMQService = rabbitMQService;
        }

        public async Task<ApiResult<GetOrderResponse>> Get(GetOrderRequest request)
        {
            await _rabbitMQService.SendEventAsync(new OrderCreatedV2Event
            {
                Order = new Order(new Repositories.Entity.OrderEntity { Id = Guid.NewGuid(), }, null),
                Id = Guid.NewGuid(),
                Name = "Jack",
            });

            var order = _repo.Get(request.Id);

            // 轉換提供外界的 DTO
            return HandleSuccess<GetOrderResponse>(new GetOrderResponse
            {
                Id = order.RootEntity.Id,
                Title = order.RootEntity.No,
                Amount = order.RootEntity.Amount,
            });
        }

        public async Task<ApiResult<CreateResponse>> Create(CreateRequest request)
        {
            // 協調業務流程(不僅限於一個 command)，如果需要，可以多個 command
            // var command = MapperProvider.Map<OrderCreatedCommand>(request);
            var command = new OrderCreatedCommand
            {
                Name = "test",
                Amount = request.Amount
            };

            var commandResult = await _mediator.Send(command);

            //return HandleSuccess<CreateResponse>(MapperProvider.Map<CreateResponse>(commandResult));
            return HandleSuccess<CreateResponse>(new CreateResponse
            {
                Id = commandResult,
            });
        }
    }
}