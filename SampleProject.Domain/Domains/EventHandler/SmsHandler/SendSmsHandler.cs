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
            var order = _repo.Get(Guid.NewGuid());

            _repo.Update(order);

            return Task.CompletedTask;
        }
    }
}