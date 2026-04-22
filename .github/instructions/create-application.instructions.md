# CreateApplications 實作規範
---
applyTo: **/SampleProject.Domain/Applications/**.Application.cs
---

## 目的
本文件說明在專案中實作 `CreateApplications` 相關功能時應遵循的規範與原則，確保程式碼的一致性、可維護性，以及符合專案既定標準。

## 適用範圍
- 適用於 `SampleProject.Domain` 層中所有 `CreateApplications` 的實作。
- 涵蓋 `Applications`、`Domains` 及 `Repositories` 等資料夾。

## 規則與規範

### 通用準則
1. **命名規則**：
   - Service 類別命名格式：`XXXApplication`（依照專案命名規範）。
   - 方法名稱應具描述性，例如：`Create`、`CreateOrder`。

2. **相依性注入**：
   - 所有相依項目一律使用建構子注入（Constructor Injection）。
   - 禁止使用靜態類別（`static class`），改以 DI Singleton 方式注入，以利單元測試 Mock。

3. **錯誤處理**：
   - 所有回應一律使用 `ApiResult<T>` 包裝。
   - 應妥善處理例外並進行記錄。

### 實作細節
1. **Command 協調**：
   - `Application` 負責協調業務流程，必要時可串接多個 Command。
   - 透過 `IMediator` 發送 Command。

2. **物件映射**：
   - 使用 `MapperProvider` 進行 Request 與 Domain 物件之間的轉換。

3. **資料驗證**：
   - 使用 `CommandValidation` 類別對輸入的 Request 進行驗證。

4. **Repository 互動**：
   - 透過定義於 `Interfaces/Repository` 的介面與 Repository 溝通。
   - Repository 的 Entity 僅作為資料庫 Raw Data，不得包含任何業務邏輯。

### 範例
```csharp
public class OrderApplication : BaseApplication, IOrderApplication
{
    private readonly IMediator _mediator;
    private readonly IOrderAggRepository _repo;

    public OrderApplication(IMediator mediator, IOrderAggRepository repo)
    {
        _mediator = mediator;
        _repo = repo;
    }

    public async Task<ApiResult<CreateResponse>> Create(CreateRequest request)
    {
        // 協調業務流程，必要時可串接多個 Command
        var command = new OrderCreatedCommand
        {
            Name = request.Name,
            Amount = request.Amount
        };

        var commandResult = await _mediator.Send(command);

        return HandleSuccess<CreateResponse>(new CreateResponse
        {
            Id = commandResult,
        });
    }
}
```

## 補充說明
- 請遵循 `.github/copilot-instructions.md` 中定義的全專案規範。
- 所有 Service 方法均須撰寫對應的單元測試。
- 測試專案使用 **NSubstitute** 進行介面 Mock。

## 相關延伸建議
- 可考慮依照本規範，為 `UpdateService` 與 `DeleteService` 建立對應的指引文件，以確保 CRUD 操作的一致性。
