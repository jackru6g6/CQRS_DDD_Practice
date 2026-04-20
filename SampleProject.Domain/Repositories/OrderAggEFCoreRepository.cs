using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Infrastructures;
using SampleProject.Domain.Interceptors.OptimisticLock.Attribute;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Repositories
{
    /// <summary>
    /// 使用 EF Core 實作訂單聚合 Repository
    /// </summary>
    public class OrderAggEFCoreRepository : BaseRepository, IOrderAggRepository
    {
        public OrderAggEFCoreRepository(IMediator mediator, SampleDbContext dbContext)
            : base(mediator, dbContext)
        {
        }

        /// <summary>
        /// 依 Id 取得訂單聚合
        /// </summary>
        [Select]
        public Order Get(Guid id)
        {
            var entity = DbContext.Orders
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);

            var items = DbContext.OrderItems
                .AsNoTracking()
                .ToList();

            return new Order(entity, items);
        }

        /// <summary>
        /// 新增訂單聚合
        /// </summary>
        [Add]
        public void Add(Order domain)
        {
            DbContext.Orders.Add(domain.RootEntity);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 更新訂單聚合
        /// </summary>
        [Update]
        public void Update(Order domain)
        {
            DbContext.Orders.Update(domain.RootEntity);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 刪除訂單聚合
        /// </summary>
        [Delete]
        public void Delete(Order domain)
        {
            var entity = DbContext.Orders.Find(domain.RootEntity.Id);
            if (entity is not null)
            {
                DbContext.Orders.Remove(entity);
                DbContext.SaveChanges();
            }
        }
    }
}
