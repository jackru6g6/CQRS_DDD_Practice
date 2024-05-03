using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Enums;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Repositories
{
    /// <summary>
    /// 要用 Singleton
    /// </summary>
    public class OrderAggRepository : IOrderAggRepository
    {
        /// <summary>
        /// 樂觀鎖
        /// </summary>
        private Dictionary<Guid, EntityState> _pessimisticLock = [];

        private List<OrderAgg> _memory = [];

        public OrderAgg? Get(Guid id)
        {
            var result = _memory.FirstOrDefault(t => t.Order.Id == id);

            if (result is not null)
            {
                if (_pessimisticLock.ContainsKey(id))
                {
                    _pessimisticLock[id] = EntityState.Unchanged; // 如果鍵存在，則更新值
                }
                else
                {
                    _pessimisticLock.Add(id, EntityState.Unchanged); // 如果鍵不存在，則添加新的鍵值對
                }
            }

            return result;
        }

        public void Add(OrderAgg order)
        {
            if (_memory.Any(t => t.Order.Id == order.Order.Id))
            {
                throw new Exception("Order Id is repeated.");
            }

            _memory.Add(order);

            _pessimisticLock.Add(order.Order.Id, EntityState.Added);
        }


        public void Update(OrderAgg order)
        {
            if (_pessimisticLock.TryGetValue(order.Order.Id, out var state) &&
                state is EntityState.Modified)
            {
                throw new Exception("Pessimistic Lock failed.");
            }

            //_memory.Remove()
            //_memory.Add()

            if (_pessimisticLock.ContainsKey(order.Order.Id))
            {
                _pessimisticLock[order.Order.Id] = EntityState.Modified;
            }
            else
            {
                _pessimisticLock.Add(order.Order.Id, EntityState.Modified);
            }
        }
    }
}