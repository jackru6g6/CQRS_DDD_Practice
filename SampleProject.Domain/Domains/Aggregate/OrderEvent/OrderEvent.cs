using SampleProject.Domain.Domains.Event.Order;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Domains.Aggregate.OrderEvent
{
    public class OrderEvent : AggregateRoot
    {
        public OrderEventEntity RootEntity { get; private set; }

        /// <summary>
        /// 初始化物建
        /// </summary>
        /// <remarks>
        /// 由於不會有取得DB的情況，故用 private constructor
        /// </remarks>
        /// <param name="rootEntity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private OrderEvent(OrderEventEntity rootEntity)
        {
            RootEntity = rootEntity ?? throw new ArgumentNullException(nameof(rootEntity));
        }

        /// <summary>
        /// 建立歷程聚合物件(訂單建立的歷程)
        /// </summary>
        /// <remarks>
        /// 利用工廠模式
        /// </remarks>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static OrderEvent Create(OrderCreatedV2Event request)
        {
            var entity = new OrderEventEntity
            {
                OrderId = request.Order.RootEntity.Id,
                Amount = request.Order.RootEntity.Amount,

                /* 
                 * 待討論議題：
                 *   這邊用訂單建立時間，或許可以考慮再 OrderCreatedV2Event 裡面加上 CreateTime?
                 *   不直接用當下時間，是因為 event 沒有順序性，有可能其他服務先處理了，導致歷程的建立時間不正確。
                 */
                CreateTime = request.Order.RootEntity.CreateTime,
            };

            return new OrderEvent(rootEntity: entity);
        }
    }
}
