using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Domains.Aggregate.Order
{
    public class OrderAgg(OrderEntity? entity) : AggregateRoot
    {
        public OrderEntity Entity { get; private set; } = entity ?? throw new Exception($"{nameof(OrderEntity)} is null.");

        public List<OrderItemEntity> Items { get; private set; } = [];

        public static OrderAgg Create(decimal amount)
        {
            var entity = new OrderEntity
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                CreateTime = DateTime.Now,
            };

            //this.AddDomainEvent(new OrderStartedEvent(this));

            return new OrderAgg(entity);
        }

        public void AddItem()
        {
            var addItme = new OrderItemEntity();
            //this.AddDomainEvent(new OrderItemStartedEvent(addItme));

            Items.Add(addItme);
            //this.AddDomainEvent(new OrderItemAddedEvent(addItme));
        }
    }
}
