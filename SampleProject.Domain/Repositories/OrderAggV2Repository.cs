using Autofac.Extras.DynamicProxy;
using SampleProject.Domain.Domains.Aggregate.Order;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Filters.OptimisticLock;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Repositories
{
    /// <summary>
    /// 要用 Singleton (因為範例的歡樂鎖存在 local memory，未來改用其他就可以不用 Singleton)
    /// </summary>
    [Intercept(typeof(SelectInterceptor))]
    public class OrderAggV2Repository : IOrderAggRepository
    {
        /// <summary>
        /// 樂觀鎖 (v2版)
        /// </summary>
        private Dictionary<string, string?> _optimisticLock = [];

        /// <summary>
        /// 替代真實資料庫
        /// </summary>
        private List<OrderAgg> _memoryDb = [];

        /// <summary>
        /// - Entity 沒有在歡樂鎖內，則寫入
        /// - Entity 有在歡樂鎖內，則表示同一時間有多服務在操作此 Entity，要小心會取得舊資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Select]
        //[SelectFilter]
        //[ServiceFilter(typeof(SelectFilterAttribute))]
        //[TypeFilter(typeof(SelectFilterAttribute))]
        public OrderAgg? Get(Guid id)
        {
            var domain = _memoryDb.FirstOrDefault(t => t.Entity.Id == id);

            // 有加入歡樂鎖才判斷
            if (domain is { Entity: IOptimisticLock })
            {
                var @lock = domain.Entity as IOptimisticLock;

                if (_optimisticLock.ContainsKey(@lock.Key))
                {
                    var oldVersion = _optimisticLock[@lock.Key];

                    if (@lock.Version == oldVersion)
                    {
                        // 情境1：版本與 _optimisticLockV2 相同，表示沒有更新，直接 return
                        return domain;
                    }
                    else
                    {
                        // 情境2：版本與 _optimisticLockV2 部同，表示有更新，需要重新取得資料，直到版本一致(可以設定 retry 次數)
                        domain = _memoryDb.FirstOrDefault(t => t.Entity.Id == id);
                        // 省略判斷版本與 retry 機制

                        return domain;
                    }
                }
                else
                {
                    // 如果鍵不存在，則添加新的鍵值對，表示這筆資料已經被讀取
                    _optimisticLock.Add(@lock.Key, @lock.Version);
                }
            }

            return domain;
        }

        /// <summary>
        /// 樂觀鎖無須做事，因為是新增
        /// </summary>
        /// <param name="domain"></param>
        /// <exception cref="Exception"></exception>
        public void Add(OrderAgg domain)
        {
            if (_memoryDb.Any(t => t.Entity.Id == domain.Entity.Id))
            {
                throw new Exception("Order Id is repeated.");
            }

            _memoryDb.Add(domain);
        }

        /// <summary>
        /// - Entity 更新歡樂鎖版本
        /// </summary>
        /// <param name="domain"></param>
        /// <exception cref="Exception"></exception>
        public void Update(OrderAgg domain)
        {
            IOptimisticLock? @lock = null;
            if (domain.Entity is IOptimisticLock)
            {
                @lock = domain.Entity;
            }

            // 判斷樂觀鎖，版本是否一致
            if (@lock is not null)
            {
                if (_optimisticLock.ContainsKey(@lock.Key))
                {
                    var oldVersion = _optimisticLock[@lock.Key];

                    if (@lock.Version == oldVersion)
                    {
                        // 情境1：版本與 _optimisticLockV2 相同，表示沒有更新，直接 update
                    }
                    else
                    {
                        // 情境2：版本與 _optimisticLockV2 部同，表示有更新，前置邏輯需要重新運作
                        //throw new Exception("OptimisticLock version is not same.");
                        throw new OptimisticLockException();
                    }
                }
                else
                {
                    // 測試用
                    throw new OptimisticLockException();

                    // 正常不會有不存在，因為 Get 時會先加入！！！
                    throw new Exception("OptimisticLock exception");
                }
            }

            // 假 DB 動作
            //_memory.Remove()
            //_memory.Add()

            // 更新歡樂鎖版本
            if (@lock is not null)
            {
                if (_optimisticLock.ContainsKey(@lock.Key))
                {
                    _optimisticLock[@lock.Key] = @lock.Version;
                }
                else
                {
                    // 正常不會有不存在，因為 Get 時會先加入！！！
                    throw new Exception("OptimisticLock exception");
                    //_optimisticLockV2.Add(@lock.Key, @lock.Version);
                }
            }
        }
    }
}