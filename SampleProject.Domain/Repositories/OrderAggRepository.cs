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
        /// 樂觀鎖 (v1版)
        /// </summary>
        private Dictionary<Guid, EntityState> _optimisticLockV1 = [];

        private List<Order> _memory = [];

        public Order? Get(Guid id)
        {
            var result = _memory.FirstOrDefault(t => t.RootEntity.Id == id);

            if (result is not null)
            {
                if (_optimisticLockV1.ContainsKey(id))
                {
                    _optimisticLockV1[id] = EntityState.Unchanged; // 如果鍵存在，則更新值
                }
                else
                {
                    _optimisticLockV1.Add(id, EntityState.Unchanged); // 如果鍵不存在，則添加新的鍵值對
                }
            }

            return result;
        }

        public void Add(Order order)
        {
            if (_memory.Any(t => t.RootEntity.Id == order.RootEntity.Id))
            {
                throw new Exception("Order Id is repeated.");
            }

            _memory.Add(order);

            _optimisticLockV1.Add(order.RootEntity.Id, EntityState.Added);
        }


        public void Update(Order order)
        {
            if (_optimisticLockV1.TryGetValue(order.RootEntity.Id, out var state) &&
                state is EntityState.Modified)
            {
                throw new Exception("Pessimistic Lock failed.");
            }

            //_memory.Remove()
            //_memory.Add()

            if (_optimisticLockV1.ContainsKey(order.RootEntity.Id))
            {
                _optimisticLockV1[order.RootEntity.Id] = EntityState.Modified;
            }
            else
            {
                _optimisticLockV1.Add(order.RootEntity.Id, EntityState.Modified);
            }
        }

        public void Delete(Order order)
        {
            _memory.RemoveAll(t => t.RootEntity.Id == order.RootEntity.Id);
        }
    }
}