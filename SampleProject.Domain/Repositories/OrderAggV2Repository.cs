using MediatR;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Interceptors.OptimisticLock.Attribute;
using SampleProject.Domain.Interfaces.Repository;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Repositories
{
    /// <summary>
    /// 要用 Singleton (因為範例的歡樂鎖存在 local memory，未來改用其他就可以不用 Singleton)
    /// </summary>
    //[Intercept(typeof(SelectInterceptor))]
    public class OrderAggV2Repository : BaseRepository, IOrderAggRepository
    {
        /// <summary>
        /// 樂觀鎖 (v2版)
        /// </summary>
        private Dictionary<string, string?> _optimisticLock = [];

        /// <summary>
        /// 替代真實資料庫
        /// </summary>
        private List<OrderEntity> _memoryDb = [];

        public OrderAggV2Repository(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// - Entity 沒有在歡樂鎖內，則寫入
        /// - Entity 有在歡樂鎖內，則表示同一時間有多服務在操作此 Entity，要小心會取得舊資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Select]
        public OrderAgg Get(Guid id)
        {
            var entity = _memoryDb.FirstOrDefault(t => t.Id == id) ?? new OrderEntity { };

            return new OrderAgg(entity);
        }

        /// <summary>
        /// 樂觀鎖無須做事，因為是新增
        /// </summary>
        /// <param name="domain"></param>
        /// <exception cref="Exception"></exception>
        public void Add(OrderAgg domain)
        {
            if (_memoryDb.Any(t => t.Id == domain.Entity.Id))
            {
                throw new Exception("Order Id is repeated.");
            }

            _memoryDb.Add(domain.Entity);

            base.SendEvent(domain);
        }

        /// <summary>
        /// - Entity 更新歡樂鎖版本
        /// </summary>
        /// <param name="domain"></param>
        [Update]
        public void Update(OrderAgg domain)
        {
            _memoryDb.Where(t => t.Id == domain.Entity.Id)
                .ToList()
                .ForEach(t =>
                {
                    t = domain.Entity;
                });

            base.SendEvent(domain);
        }

        [Delete]
        public void Delete(OrderAgg order)
        {
            _memoryDb.RemoveAll(t => t.Id == order.Entity.Id);

            base.SendEvent(order);
        }
    }
}