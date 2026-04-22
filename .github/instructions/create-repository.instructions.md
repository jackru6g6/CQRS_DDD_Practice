---
description: "Repository 撰寫規範，適用於 SampleProject.Domain.Repositories。"
applyTo: "SampleProject.Domain/Repositories/**/*.cs"
---

# Repository 撰寫規範

## 基本原則

1. **依賴注入**：Repository 必須透過建構函式注入必要的相依物件，例如 `DbContext` 和 `IMediator`。
2. **方法標註**：使用自訂 Attribute（如 `[Select]`、`[Add]`）標註方法，清楚表達方法的用途。
3. **聚合根操作**：
   - Repository 僅操作聚合根（Aggregate Root），不直接操作子物件。
   - 聚合根的操作應包含完整的業務邏輯。
4. **資料庫操作**：
   - 使用 EF Core 的 `DbContext` 進行資料存取。
   - 須使用 `AsNoTracking()` 提升查詢效能，除非需要追蹤。
5. **例外處理**：
   - Repository 方法不應捕捉例外，應將例外拋出由上層處理。

---

## 命名規範

| 類型       | 命名規則                     |
|------------|------------------------------|
| Repository | `{聚合名稱}AggEFCoreRepository` |

---

## 範例

以下為 `OrderAggEFCoreRepository` 的範例：

```csharp
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
```