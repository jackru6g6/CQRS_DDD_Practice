using MediatR;

namespace SampleProject.Domain.Domains.Event.Order
{
    public class OrderCreatedV2Event : INotification
    {
        /// <summary>
        /// 訂單物件
        /// </summary>
        public Aggregate.Order.Order Order { get; init; }
    }
}
