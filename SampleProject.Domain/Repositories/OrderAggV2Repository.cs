﻿using MediatR;
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
        private List<OrderEntity> _memoryDb = [new OrderEntity { Id = Guid.Parse("E705D262-B82D-40E5-AE94-C16B5CBDBAE1"), Amount = 99.12m }];

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
        public Order Get(Guid id)
        {
            var entity = _memoryDb.FirstOrDefault(t => t.Id == id);

            return new Order(entity, null);
        }

        /// <summary>
        /// 樂觀鎖無須做事，因為是新增
        /// </summary>
        /// <param name="domain"></param>
        /// <exception cref="Exception"></exception>
        public void Add(Order domain)
        {
            if (_memoryDb.Any(t => t.Id == domain.RootEntity.Id))
            {
                throw new Exception("Order Id is repeated.");
            }

            _memoryDb.Add(domain.RootEntity);

            base.SendEvent(domain);
        }

        /// <summary>
        /// - Entity 更新歡樂鎖版本
        /// </summary>
        /// <param name="domain"></param>
        [Update]
        public void Update(Order domain)
        {
            _memoryDb.Where(t => t.Id == domain.RootEntity.Id)
                .ToList()
                .ForEach(t =>
                {
                    t = domain.RootEntity;
                });

            base.SendEvent(domain);
        }

        [Delete]
        public void Delete(Order order)
        {
            _memoryDb.RemoveAll(t => t.Id == order.RootEntity.Id);

            base.SendEvent(order);
        }
    }
}