using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Domains.Aggregate.Order
{
    public class Order : AggregateRoot
    {
        public OrderEntity RootEntity { get; private set; }

        public List<OrderItemEntity> Items { get; private set; } = [];

        public Order(OrderEntity? entity, IEnumerable<OrderItemEntity>? items)
        {
            RootEntity = entity ?? throw new EntityNullException(nameof(OrderEntity));
            Items = items?.ToList();
        }

        /// <summary>
        /// 建立訂單聚合物件
        /// </summary>
        /// <remarks>
        /// 利用工廠模式
        /// </remarks>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Order Create(OrderCreatedCommand request)
        {
            var entity = new OrderEntity
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                CreateTime = DateTime.Now,
            };

            var result = new Order(entity, null);
            result.AddDomainEvent(new OrderCreatedEvent { Id = entity.Id, });

            return result;
        }

        public void AddItem()
        {
            var ItmeAdded = new OrderItemEntity();
            Items.Add(ItmeAdded);

            AddDomainEvent(new OrderItemAddedEvent { Id = RootEntity.Id });
        }
    }
}
