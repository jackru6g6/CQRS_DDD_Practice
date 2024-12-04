using MediatR;

namespace SampleProject.Domain.Domains.Event.Order
{
    public record OrderCreatedEvent : INotification
    {
        /// <summary>
        /// 訂單 Id
        /// </summary>
        public Guid Id { get; init; }
    }
}