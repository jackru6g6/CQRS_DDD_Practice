using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Domains.Aggregate.Order
{
    public class OrderAgg(OrderEntity? entity) : AggregateRoot
    {
        public OrderEntity Entity { get; private set; } = entity ?? throw new Exception($"{nameof(OrderEntity)} is null.");

        public List<OrderItemEntity> Items { get; private set; } = [];

        /// <summary>
        /// 建立訂單聚合物件
        /// </summary>
        /// <remarks>
        /// 利用工廠模式
        /// </remarks>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static OrderAgg Create(OrderCreatedCommand request)
        {
            var entity = new OrderEntity
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                CreateTime = DateTime.Now,
            };

            var result = new OrderAgg(entity);
            result.AddDomainEvent(new OrderCreatedEvent
            {
                Id = entity.Id,
            });

            return result;
        }

        public void AddItem()
        {
            var addItme = new OrderItemEntity();
            //this.AddDomainEvent(new OrderItemStartedEvent(addItme));

            Items.Add(addItme);

            AddDomainEvent(new OrderItemAddedEvent { Id = Entity.Id });
        }
    }
}
