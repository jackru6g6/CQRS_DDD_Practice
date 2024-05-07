using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Domains.Aggregate.Order
{
    public class OrderAgg : AggregateRoot
    {
        public OrderEntity Order { get; private set; }

        public List<OrderItemEntity> Items { get; private set; } = [];

        public OrderAgg(decimal amount)
        {
            Order = new OrderEntity
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                CreateTime = DateTime.Now,
            };

            //this.AddDomainEvent(new OrderStartedEvent(this));
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
