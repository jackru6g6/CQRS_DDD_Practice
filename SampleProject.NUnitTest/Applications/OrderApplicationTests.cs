using MediatR;
using NSubstitute;
using SampleProject.API.Model.Order.Request;
using SampleProject.Domain.Applications;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Interfaces.Repository;
using SampleProject.Domain.Repositories.Entity;
using SampleProject.Domain.Interfaces.Domain.Service;

namespace SampleProject.NUnitTest.Applications
{
    [TestFixture]
    public class OrderApplicationTests
    {
        private IMediator _mediator;
        private IOrderAggRepository _repo;
        private IRabbitMQService _rabbitMQService;
        private OrderApplication _sut;

        [SetUp]
        public void SetUp()
        {
            _mediator = Substitute.For<IMediator>();
            _repo = Substitute.For<IOrderAggRepository>();
            _rabbitMQService = Substitute.For<IRabbitMQService>();
            _sut = new OrderApplication(_mediator, _repo, _rabbitMQService);
        }

        #region Get

        [Test]
        public async Task Get_有效Id_回傳成功結果()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var entity = new OrderEntity
            {
                Id = orderId,
                No = "ORD-001",
                Amount = 500m,
            };
            var order = new Order(entity, null);
            _repo.Get(orderId).Returns(order);

            var request = new GetOrderRequest { Id = orderId };

            // Act
            var result = await _sut.Get(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSucceed, Is.True);
            Assert.That(result.Data!.Id, Is.EqualTo(orderId));
            Assert.That(result.Data!.Amount, Is.EqualTo(500m));
        }

        [Test]
        public void Get_Repository回傳空Entity_拋出EntityNullException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _repo.Get(orderId).Returns(_ => throw new EntityNullException(nameof(OrderEntity)));

            var request = new GetOrderRequest { Id = orderId };

            // Act & Assert
            Assert.ThrowsAsync<EntityNullException>(() => _sut.Get(request));
        }

        #endregion

        #region Create

        [Test]
        public async Task Create_有效Request_回傳成功結果()
        {
            // Arrange
            var newId = Guid.NewGuid();
            _mediator.Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>())
                     .Returns(newId);

            var request = new CreateRequest { Amount = 1000m };

            // Act
            var result = await _sut.Create(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSucceed, Is.True);
            Assert.That(result.Data!.Id, Is.EqualTo(newId));
        }

        [Test]
        public async Task Create_MediatR被呼叫一次()
        {
            // Arrange
            _mediator.Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>())
                     .Returns(Guid.NewGuid());

            var request = new CreateRequest { Amount = 200m };

            // Act
            await _sut.Create(request);

            // Assert：確認 Send 只被呼叫一次
            await _mediator.Received(1).Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>());
        }

        #endregion
    }
}
