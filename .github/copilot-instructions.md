# Copilot 專案規則

## 專案結構

本解決方案由以下四個專案組成：

- **SampleProject.API**：對外 API 層，包含 Controllers、Middlewares、Filters、Extensions、Presenters、Settings。
- **SampleProject.API.Model**：對外 API 的 Request / Response Model，可作為 NuGet 套件對外發布，不得包含任何 `enum` 型別，一律改為基礎型別（如 `int`、`string`）。
- **SampleProject.Domain**：核心業務邏輯層，包含 Application、Domain、Repository、Infrastructure、Extension、Exception、Enum、Interface 等。
- **SampleProject.NUnitTest**：單元測試專案，使用 NSubstitute 進行 Mock。


## 專案參考方向（由外到內，禁止反向）

```
SampleProject.API → SampleProject.Domain → SampleProject.API.Model
SampleProject.NUnitTest → SampleProject.Domain
```

> 層與層之間只能由上而下依賴，禁止由下而上，也禁止同層平行依賴（避免 ADP 問題）。

## 命名規則

| 類型 | 命名規則 |
|------|----------|
| Request Model | `XXXRequest` |
| Response Model | `XXXResponse` |
| Interface | `IXXX` |
| Application Service | `XXXApplication` |
| Repository | `XXXRepository` |
| Entity（資料庫原始資料） | `XXXEntity` |
| API 資料格式 | `XXXApiData` |
| Domain Service | `XXXService` |
| Extension 類別 | `XXXExtension` |
| Attribute | `XXXAttribute` |

## 程式碼撰寫規則

1. **註解**：`property`、`field`、`const`、`enum` 必須加上 XML 註解，方法（function）可視情況加。
2. **API.Model 不可使用 enum**：`SampleProject.API.Model` 專案中所有型別一律使用基礎型別（`int`、`string` 等）取代 `enum`。
3. **禁止靜態類別（static class）**：改用一般類別搭配 .NET Core DI 注入為 Singleton，以利單元測試 Mock。例外：加解密等異動性低的 InfraLib 工具類別可繼續使用 `static`。
4. **Application 職責**：`XXXApplication` 負責訊息驗證、錯誤處理、認證授權、呼叫 Domain Service 完成業務需求、以及 Request/Response 的輸入輸出轉型。
5. **Domain Model 不依賴外部**：Domain Model 不得依賴任何外部類別或服務，欄位基本為私有（`private field`），需要開放才使用 `property`。
6. **Domain Service 職責**：與一個或多個 Repository 溝通，將 Entity 物件轉型為 Domain Model。
7. **Repository Entity 無邏輯**：Entity 只作為資料庫 Raw Data，不得包含任何業務邏輯。
8. **多語系**：內部訊息可用中文；若為對外 External Gateway 的 ApiResponse，需提供對應英文訊息。
9. **Rich Domain Model**：本專案採用 DDD 開發，除 DTO（`XXXRequㄖest`、`XXXResponse`、`XXXEntity`、`XXXApiData`、ViewModel）之外，所有 Domain Model 一律為 **Rich Domain Model**，規則如下：
   - Domain Model 的欄位（field）必須為 `private`，不得對外直接賦值。
   - 所有狀態變更須透過明確命名的方法（如 `PlaceOrder()`、`Cancel()`）封裝業務邏輯，禁止在 Domain Model 外部直接修改狀態。
   - 建構子（constructor）應負責必要欄位的初始化與不變式（invariant）驗證，確保 Domain Model 建立後即處於合法狀態。
   - 集合型別（如 `List<T>`）對外只開放 `IReadOnlyCollection<T>`，避免外部直接操作集合。
   - 避免使用貧血模型（Anemic Domain Model），業務規則不得外漏至 Application 或 Service 層。

## 單元測試規則

- 測試專案使用 **NSubstitute** 進行介面 Mock。
- 靜態類別不易測試，因此業務邏輯類別一律使用 DI Singleton，方便 Mock 替換。
