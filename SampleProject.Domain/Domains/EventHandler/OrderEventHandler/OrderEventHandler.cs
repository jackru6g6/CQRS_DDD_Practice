using MediatR;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Domains.Aggregate.OrderEvent;
using SampleProject.Domain.Domains.Event.Order;

namespace SampleProject.Domain.Domains.EventHandler.RiskHandler
{
    public class OrderEventHandler : INotificationHandler<OrderCreatedV2Event>
    {
        public OrderEventHandler() { }

        [RetryEvent]
        public Task Handle(OrderCreatedV2Event notification, CancellationToken cancellationToken)
        {
            // 風險控管
            var @event = OrderEvent.Create(notification);

            // save db，懶得實作

            return Task.CompletedTask;
        }
    }
}