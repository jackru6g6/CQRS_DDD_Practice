using MediatR;
using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;
using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Interfaces.Application;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Applications
{
    public class OrderApplication : BaseApplication, IOrderApplication
    {
        private readonly IMediator _mediator;
        private readonly IOrderAggRepository _repo;

        public OrderApplication(IMediator mediator, IOrderAggRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        public async Task<ApiResult<GetOrderResponse>> Get(GetOrderRequest request)
        {
            var order = _repo.Get(request.Id);

            // 轉換提供外界的 DTO
            return HandleSuccess<GetOrderResponse>(new GetOrderResponse
            {
                Id = order.Entity.Id,
                Title = order.Entity.No,
                Amount = order.Entity.Amount,
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