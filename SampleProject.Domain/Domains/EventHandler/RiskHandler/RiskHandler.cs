using MediatR;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Domains.EventHandler.RiskHandler
{
    public class RiskHandler : INotificationHandler<OrderCreatedEvent>
                             , INotificationHandler<OrderItemAddedEvent>
    {
        private readonly IOrderAggRepository _orderRepo;

        public RiskHandler(IOrderAggRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        [RetryEvent]
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            // 風險控管
            var order = _orderRepo.Get(notification.Id);
            order.AddItem();

            _orderRepo.Update(order);

            return Task.CompletedTask;
        }

        [RetryEvent]
        public Task Handle(OrderItemAddedEvent notification, CancellationToken cancellationToken)
        {
            // 測試

            return Task.CompletedTask;
        }
    }
}