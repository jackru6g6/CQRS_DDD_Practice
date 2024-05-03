using MediatR;
using SampleProject.Domain.Domains.Event.Order;

namespace SampleProject.Domain.Domains.EventHandler.SmsHandler
{
    public class SendSmsHandler : INotificationHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}