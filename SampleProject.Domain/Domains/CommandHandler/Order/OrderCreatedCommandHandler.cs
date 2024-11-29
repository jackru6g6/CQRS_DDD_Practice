using MediatR;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Domains.CommandHandler.Order
{
    public class OrderCreatedCommandHandler : IRequestHandler<OrderCreatedCommand, Guid>
    {
        private readonly IOrderAggRepository _orderRepo;

        public OrderCreatedCommandHandler(IOrderAggRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public Task<Guid> Handle(OrderCreatedCommand request, CancellationToken cancellationToken)
        {
            _orderRepo.Get(Guid.NewGuid());

            var orderAgg = new OrderAgg(request.Amount);
            _orderRepo.Add(orderAgg);

            return Task.FromResult(orderAgg.Entity.Id);
        }
    }
}