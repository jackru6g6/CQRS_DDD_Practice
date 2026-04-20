using MediatR;
using System.Text.Json.Serialization;

namespace SampleProject.Domain.Domains.Event.Order
{
    public class OrderCreatedV2Event : INotification
    {
        /// <summary>
        /// 訂單物件
        /// </summary>
        //[JsonIgnore]
        public Aggregate.Order.Order Order { get; init; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
