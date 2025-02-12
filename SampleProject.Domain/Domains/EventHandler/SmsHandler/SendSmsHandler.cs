using MediatR;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Domains.Event.Order;

namespace SampleProject.Domain.Domains.EventHandler.SmsHandler
{
    public class SendSmsHandler : INotificationHandler<OrderCreatedEvent>
    {
        public SendSmsHandler() { }

        [RetryEvent]
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {


            return Task.CompletedTask;
        }
    }
}