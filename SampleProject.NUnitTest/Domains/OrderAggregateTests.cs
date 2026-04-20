using NUnit.Framework;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.NUnitTest.Domains
{
    [TestFixture]
    public class OrderAggregateTests
    {
        #region 建構子

        [Test]
        public void Constructor_有效Entity_建立成功()
        {
            // Arrange
            var entity = new OrderEntity { Id = Guid.NewGuid(), Amount = 100m };

            // Act
            var order = new Order(entity, null);

            // Assert
            Assert.That(order.RootEntity, Is.Not.Null);
            Assert.That(order.RootEntity.Id, Is.EqualTo(entity.Id));
            Assert.That(order.Items, Is.Empty);
        }

        [Test]
        public void Constructor_NullEntity_拋出EntityNullException()
        {
            // Act & Assert
            Assert.Throws<EntityNullException>(() => new Order(null, null));
        }

        [Test]
        public void Constructor_帶有Items_Items正確初始化()
        {
            // Arrange
            var entity = new OrderEntity { Id = Guid.NewGuid(), Amount = 300m };
            var items = new List<OrderItemEntity>
            {
                new OrderItemEntity(),
                new OrderItemEntity(),
            };

            // Act
            var order = new Order(entity, items);

            // Assert
            Assert.That(order.Items.Count, Is.EqualTo(2));
        }

        #endregion

        #region Create 工廠方法

        [Test]
        public void Create_有效Command_回傳新訂單且有DomainEvent()
        {
            // Arrange
            var command = new OrderCreatedCommand { Name = "測試訂單", Amount = 999m };

            // Act
            var order = Order.Create(command);

            // Assert
            Assert.That(order, Is.Not.Null);
            Assert.That(order.RootEntity.Amount, Is.EqualTo(999m));
            Assert.That(order.DomainEvents, Is.Not.Null);
            Assert.That(order.DomainEvents.Count, Is.EqualTo(1));
        }

        [Test]
        public void Create_每次呼叫_產生不同Id()
        {
            // Arrange
            var command = new OrderCreatedCommand { Name = "訂單A", Amount = 100m };

            // Act
            var order1 = Order.Create(command);
            var order2 = Order.Create(command);

            // Assert
            Assert.That(order1.RootEntity.Id, Is.Not.EqualTo(order2.RootEntity.Id));
        }

        #endregion

        #region AddItem

        [Test]
        public void AddItem_加入一筆明細_Items數量增加且產生DomainEvent()
        {
            // Arrange
            var entity = new OrderEntity { Id = Guid.NewGuid(), Amount = 500m };
            var order = new Order(entity, null);

            // Act
            order.AddItem();

            // Assert
            Assert.That(order.Items.Count, Is.EqualTo(1));
            Assert.That(order.DomainEvents, Is.Not.Null);
            Assert.That(order.DomainEvents.Any(), Is.True);
        }

        #endregion
    }
}
