﻿using MediatR;
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

        public Task<Guid> Handle(OrderCreatedCommand command, CancellationToken cancellationToken)
        {
            var orderAgg = Aggregate.Order.Order.Create(command);
            _orderRepo.Add(orderAgg);

            return Task.FromResult(orderAgg.RootEntity.Id);
        }
    }
}