using Castle.DynamicProxy;
using SampleProject.Domain.Exceptions;
using SampleProject.Domain.Interceptors.OptimisticLock.Attribute;
using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Filters.OptimisticLock
{
    public class OptimisticLockInterceptor : IInterceptor
    {
        /// <summary>
        /// 樂觀鎖
        /// </summary>
        private Dictionary<string, string?> _optimisticLock = [];

        public void Intercept(IInvocation invocation)
        {
            var attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true);

            var selectAttribute = attributes.OfType<SelectAttribute>().FirstOrDefault();
            var updateAttribute = attributes.OfType<UpdateAttribute>().FirstOrDefault();
            var deleteAttribute = attributes.OfType<DeleteAttribute>().FirstOrDefault();

            if (selectAttribute is not null)
            {
                HandleSelect(invocation);

                return;
            }

            if (updateAttribute is not null)
            {
                // TODO：redlock
                HandleUpdate(invocation);

                return;
            }

            if (deleteAttribute is not null)
            {
                // TODO：redlock
                HandleDelete(invocation);

                return;
            }

            // 預設行為
            invocation.Proceed();
        }

        private void HandleSelect(IInvocation invocation)
        {
            invocation.Proceed();

            var @lock = GetLock(invocation.ReturnValue);
            if (@lock is not null)
            {
                AddLock(@lock);
            }
        }

        private void HandleUpdate(IInvocation invocation)
        {
            var @lock = GetLock(invocation.Arguments);

            if (@lock is null)
            {
                // 無鎖時直接執行方法
                invocation.Proceed();
            }
            else
            {
                // 檢查鎖是否正確
                CheckLock(@lock);

                invocation.Proceed();

                // 更新鎖
                UpdateLock(@lock);
            }
        }

        private void HandleDelete(IInvocation invocation)
        {
            var @lock = GetLock(invocation.Arguments);

            if (@lock is null)
            {
                // 無鎖時直接執行方法
                invocation.Proceed();
            }
            else
            {
                // 檢查鎖是否正確
                CheckLock(@lock);

                invocation.Proceed();

                // 刪除鎖
                DeleteLock(@lock);
            }
        }

        private void AddLock(IOptimisticLock @lock)
        {
            if (_optimisticLock.ContainsKey(@lock.Key))
            {
                var oldVersion = _optimisticLock[@lock.Key];

                if (@lock.Version == oldVersion)
                {
                    // 情境1：版本與 _optimisticLockV2 相同，表示沒有更新，直接 return
                    return;
                }
                else
                {
                    // 情境2：版本與 _optimisticLockV2 部同，表示有更新，需要重新取得資料，直到版本一致(可以設定 retry 次數)
                    throw new Exception("資料不一致，需要重新取得一次資料");
                }
            }
            else
            {
                // 如果鍵不存在，則添加新的鍵值對，表示這筆資料已經被讀取
                _optimisticLock.Add(@lock.Key, @lock.Version);
            }
        }

        private void CheckLock(IOptimisticLock @lock)
        {
            // 判斷樂觀鎖，版本是否一致
            if (_optimisticLock.ContainsKey(@lock.Key))
            {
                var oldVersion = _optimisticLock[@lock.Key];

                if (@lock.Version == oldVersion)
                {
                    // 情境1：版本與 _optimisticLockV2 相同，表示沒有更新，直接 update
                    return;
                }
                else
                {
                    // 情境2：版本與 _optimisticLockV2 部同，表示有更新，前置邏輯需要重新運作
                    throw new OptimisticLockException();
                }
            }
            else
            {
                // 正常會有值，如果沒有表示此筆資料被其他服務執行"刪除"，需要重新跑商務邏輯   
                throw new OptimisticLockException();
            }
        }

        private void UpdateLock(IOptimisticLock @lock)
        {
            if (_optimisticLock.ContainsKey(@lock.Key))
            {
                _optimisticLock[@lock.Key] = @lock.Version;
            }
            else
            {
                // 正常不會有不存在，因為 Get 時會先加入！！！
                //throw new Exception("OptimisticLock exception");

                _optimisticLock.Add(@lock.Key, @lock.Version);
            }
        }

        private void DeleteLock(IOptimisticLock @lock)
        {
            _optimisticLock.Remove(@lock.Key);
        }

        private IOptimisticLock? GetLock(object? data)
        {
            if (data is null)
            {
                return null;
            }

            var lockProperty = data.GetType()
                                   .GetProperties()
                                   .FirstOrDefault(p => p.PropertyType.GetInterface(nameof(IOptimisticLock)) is not null);
            if (lockProperty is not null)
            {
                var lockValue = lockProperty.GetValue(data) as IOptimisticLock;
                if (lockValue is not null)
                {
                    return lockValue;
                }
            }

            return null;
        }
    }
}