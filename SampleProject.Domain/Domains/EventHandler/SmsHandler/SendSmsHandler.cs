using MediatR;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Domains.EventHandler.SmsHandler
{
    public class SendSmsHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IOrderAggRepository _repo;

        public SendSmsHandler(IOrderAggRepository repo)
        {
            _repo = repo;
        }

        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _repo.Get(Guid.NewGuid());
            _repo.Update(new Aggregate.Order.OrderAgg(123.1m) { });

            return Task.CompletedTask;
        }
    }
}