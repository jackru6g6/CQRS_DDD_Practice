---
description: "Use when creating DDD domain services, including Aggregate, Command, CommandHandler, CommandValidation, Event, EventHandler, ValueObject in SampleProject.Domain. 適用於建立 DDD 領域服務，包含聚合根、命令、命令處理器、命令驗證、領域事件、事件處理器、值物件。"
applyTo:"SampleProject.Domain/Domains/**"
---

# DDD Domain 服務建立規範

## 目錄結構

所有 Domain 元件依類型放入對應資料夾，並依業務聚合（如 `Order`）再建子目錄：

```
SampleProject.Domain/Domains/
├── Aggregate/
│   └── {聚合名稱}/
│       └── {聚合名稱}.cs               ← 聚合根
├── Command/
│   └── {聚合名稱}/
│       └── {聚合名稱}{動作}Command.cs  ← 命令
├── CommandHandler/
│   └── {聚合名稱}/
│       └── {聚合名稱}{動作}CommandHandler.cs
├── CommandValidation/
│   └── {聚合名稱}/
│       └── {聚合名稱}{動作}CommandValidator.cs
├── Event/
│   └── {聚合名稱}/
│       └── {聚合名稱}{動作}Event.cs    ← 領域事件
├── EventHandler/
│   └── {Handler名稱}/
│       └── {Handler名稱}.cs
└── ValueObject/
    └── {值物件名稱}.cs
```

---

## 1. Aggregate（聚合根）

### 命名規則
- 類別名稱：`{聚合名稱}`（如 `Order`）
- 必須繼承 `AggregateRoot`

### 規範
- 欄位一律為 `private`；僅需開放才用 `property`
- 建立方法使用靜態工廠方法 `Create()`，禁止公開建構子直接 new
- 業務行為改變狀態時，同步呼叫 `AddDomainEvent()` 發佈領域事件
- 聚合根不得依賴任何外部 Service 或 Repository

### 範例
```csharp
public class Order : AggregateRoot
{
    /// <summary>訂單主實體</summary>
    public OrderEntity RootEntity { get; private set; }

    /// <summary>訂單項目清單</summary>
    public List<OrderItemEntity> Items { get; private set; } = new();

    private Order() { }

    /// <summary>建立訂單聚合</summary>
    public static Order Create(string name, decimal amount)
    {
        var entity = new OrderEntity
        {
            Id = Guid.NewGuid().ToString(),
            No = name,
            Amount = amount,
            CreateTime = DateTime.UtcNow,
        };

        var order = new Order { RootEntity = entity };
        order.AddDomainEvent(new OrderCreatedEvent(order));
        return order;
    }
}
```

---

## 2. Command（命令）

### 命名規則
- 類別名稱：`{聚合名稱}{動作}Command`（如 `OrderCreatedCommand`）
- 使用 `record` 型別，實作 `IRequest<TResponse>`

### 規範
- 僅包含輸入資料，不含任何業務邏輯
- 所有屬性需加上 XML 註解

### 範例
```csharp
/// <summary>建立訂單命令</summary>
public record OrderCreatedCommand : IRequest<string>
{
    /// <summary>訂單名稱</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>訂單金額</summary>
    public decimal Amount { get; init; }
}
```

---

## 3. CommandHandler（命令處理器）

### 命名規則
- 類別名稱：`{聚合名稱}{動作}CommandHandler`
- 實作 `IRequestHandler<TCommand, TResponse>`

### 規範
- 職責：呼叫聚合根工廠方法建立聚合，透過 Repository 持久化，回傳結果
- 不得直接執行資料庫操作；一律透過 Repository 介面
- 使用建構子注入相依性

### 範例
```csharp
public class OrderCreatedCommandHandler : IRequestHandler<OrderCreatedCommand, string>
{
    private readonly IOrderAggRepository _repo;
    private readonly IValidator<OrderCreatedCommand> _validator;

    public OrderCreatedCommandHandler(
        IOrderAggRepository repo,
        IValidator<OrderCreatedCommand> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<string> Handle(OrderCreatedCommand request, CancellationToken cancellationToken)
    {
        _validator.Validate(request);

        var order = Order.Create(request.Name, request.Amount);
        _repo.Add(order);

        return order.RootEntity.Id;
    }
}
```

---

## 4. CommandValidation（命令驗證）

### 命名規則
- 類別名稱：`{聚合名稱}{動作}CommandValidator`
- 實作 `IValidator<TCommand>`

### 規範
- 驗證業務規則；邏輯驗證失敗拋出對應的 Domain Exception
- 可依賴 Repository 或外部 Service（透過建構子注入）以進行需要查詢資料的業務驗證

### 範例
```csharp
public class OrderCreatedCommandValidator : IValidator<OrderCreatedCommand>
{
    public void Validate(OrderCreatedCommand request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("金額必須大於 0");
    }
}
```

---

## 5. Event（領域事件）

### 命名規則
- 類別名稱：`{聚合名稱}{動作}Event`（如 `OrderCreatedEvent`）
- 使用 `record` 型別，實作 `INotification`

### 規範
- 僅攜帶事件發生當下所需的資料（聚合根快照或關鍵 ID）
- 不含任何業務邏輯

### 範例
```csharp
/// <summary>訂單建立事件</summary>
public record OrderCreatedEvent(Order Order) : INotification;
```

---

## 6. EventHandler（事件處理器）

### 命名規則
- 類別名稱：`{業務名稱}Handler`（如 `RiskHandler`、`SendSmsHandler`）
- 實作 `INotificationHandler<TEvent>`
- 依業務職責分資料夾，一個 Handler 可處理多個事件

### 規範
- 每個 Handler 只負責單一業務面向（風控、簡訊、歷程紀錄等）
- 需要重試邏輯時加上 `[RetryEvent]` Attribute
- 使用建構子注入相依性

### 範例
```csharp
public class RiskHandler :
    INotificationHandler<OrderCreatedEvent>,
    INotificationHandler<OrderItemAddedEvent>
{
    private readonly IOrderAggRepository _repo;

    public RiskHandler(IOrderAggRepository repo)
    {
        _repo = repo;
    }

    [RetryEvent]
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        // 風控業務邏輯
    }

    public async Task Handle(OrderItemAddedEvent notification, CancellationToken cancellationToken)
    {
        // 風控業務邏輯
    }
}
```

---

## 7. ValueObject（值物件）

### 命名規則
- 類別名稱：業務名稱即可（如 `PhoneNumber`、`Money`）
- 繼承抽象類別 `ValueObject`，或使用 `record` 型別

### 規範
- 不可變（immutable）；所有屬性均為 `init` 或 `readonly`
- 必須實作 `GetEqualityComponents()` 以確保值相等語意
- 包含自身格式驗證邏輯（建構子或靜態工廠方法內）

### 範例
```csharp
public class PhoneNumber : ValueObject
{
    /// <summary>國碼</summary>
    public string AreaCode { get; }

    /// <summary>號碼</summary>
    public string Number { get; }

    public PhoneNumber(string areaCode, string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("電話號碼不可為空");

        AreaCode = areaCode;
        Number = number;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AreaCode;
        yield return Number;
    }
}
```

---

## 通用規範

| 項目 | 規則 |
|------|------|
| 禁止靜態類別 | 所有 Domain 元件改以 DI Singleton 注入 |
| XML 註解 | `property`、`field`、`const`、`enum` 必須加上 XML 註解 |
| Domain 不依賴外部 | Domain Model 不得依賴任何 Application、Repository 或 Infrastructure |
| Entity 無邏輯 | `XXXEntity` 僅為資料庫 Raw Data，不可含業務邏輯 |
| 單元測試 | 每個 CommandHandler、CommandValidator、EventHandler 均需有對應測試 |
