# SampleProj 專案架構

## 專案結構

- SampleProj.Api：API 部分。

  1. Controllers
  2. Properties
  3. Examples：Swagger 範例
  4. Extensions
  5. Filters
  6. Middlewares
  7. Presenters
  8. Settings：appsettings.json 屬性映射

- SampleProj.Api.Domain：API 專用 Requser、Response Model，供對外 nuget 參考。

  1. Models：傳入傳出參數定義，DTO

     > 作用：
     >
     > - 需求與回應 Model
     >
     > 命名規則：
     >
     > - 需求物件 XXX<font color=red>**Request**</font>
     > - 回傳物件 XXX<font color=red>**Response**</font>

- SampleProj.Domain：Domain、Service(Application)、Repository 使用與實做。

  1. Consts：常量變數定義

     > 作用：
     >
     > - Api 回應定義
     >
     > - 常數定義
     >
     > 命名規則：無

  2. Enums：內部邏輯列舉定義

     > 作用：
     >
     > - 列舉定義
     >
     > 注意：
     >
     > - 假如內部共同使用，則會移至 Apollo.Common 
     >
     > 命名規則：無

  3. Interfaces：全部介面定定義

     > 作用：
     >
     > - 所有 Interface 定義
     >
     > 命名規則：<font color=red>**I**</font>XXX

  4. Application：`Application`，理論上會有下列功能

     > 作用：
     >
     > - 消息驗證
     > - 錯誤處理
     > - 監控
     > - 認證與授權
     > - 操作 Domain Service 去完成業務需求
     > - Request、Response 輸出輸入參數轉型
     >
     > 命名規則：XXX<font color=red>**AppService**</font> 

  5. Infrastructures：`Infrastructures`，各種基礎建設實作 ( like：Network、Redis、MQ... )，專案有需求才會實作

     > 作用：
     >
     > - 基礎設施技術實作 (like:API、Cache、Notity)
     >
     > 命名規則：無

  6. Repositories：`Infrastructures` 實踐資料存取

     > 作用：
     >
     > - 引用 api、cache、db 進行存取資料
     >
     > 命名規則：XXX<font color=red>**Repository**</font>

     1. Entity：Raw Data (裡面不會有邏輯)

        > 作用：
        >
        > - 資料庫 Raw Data (不會有邏輯)
        >
        > 命名規則：XXX<font color=red>**Entity**</font>

     2. ApiData：API 輸入與回傳資料格式

        > 作用：
        >
        > - 呼叫 Api 輸入、回傳參數格式
        >
        > 命名規則：XXX<font color=red>**ApiData**</font>

  7. Domains：`Domain`

     1. Model：(domain model) 領域業務邏輯 (不該依賴任何東西)

        > 作用：
        >
        > - 領域邏輯
        >
        > 內容：
        >
        > - 有主要 Main Model (Repository/Entity)
        > - 次要[可有可無] Deeper Model (Repository/Entity)
        >
        > 注意：
        >
        > - 裡面不可依賴其他東西
        > - 基本都是私有 Field，需要開放才用 Property
        >
        > 命名規則：無

     2. Service：(domain service) 複雜或可複用邏輯，主要與 Repository 溝通存取資料庫，並處理邏輯 (需要將 Repository 資料轉型為 domain model)

        > 作用：
        >
        > - 商務邏輯
        >
        > 內容：
        >
        > - 與 一個或多個 Repository 溝通
        > - 將 Entity 物件轉型 Model
        >
        > 命名規則：XXX<font color=red>**Service**</font>

  8. Extensions：擴充，各類 DI 設定( like：AutoMapper)

     > 作用：
     >
     > - 實作擴展物件 (like:AutoMapper)
     >
     > 命名規則：XXX<font color=red>**Extension**</font>

  9. Exceptions：自訂例外錯

     > 命名規則：無

  10. Attributes：自定義 attribute

      > 命名規則：XXX<font color=red>**Attribute**</font>

- SampleProj.NUnitTest：測試專案。

  - 使用套件：NSubstitute

    [Callbacks, void calls and When..Do](https://nsubstitute.github.io/help/callbacks/)

---

### 專案參考：(外到內)

- SampleProj.Api → SampleProj.Domain → SampleProj.Api.Domain
- SampleProj.NUnitTest → SampleProj.Domain



## 撰寫規則

1. property、field、const、enum 必須註解，方法(function)可有可無。
2. Api.Domain 專案，不會有任何 enum 型別，一律改為基礎型別。
3. 層與層之間的應用必須由上而下，禁止由下而上，也不可同層平行依賴(可能造成ADP)。

[【元件如何正確使用 ?】元件耦合性三大原則 : ADP、SDP、SAP - iT 邦幫忙::一起幫忙解決難題，拯救 IT 人的一天](https://ithelp.ithome.com.tw/articles/10254804)

3. 靜態類別(static class) 改為一般類別，使用 單例 Singleton ( .net core 內建注入) 實作。

   1. 好處：方便撰寫單元測試，靜態類別不好模擬建立 Instance，可以用 mock 模擬，但很麻煩。
   2. Apollo.InfraLib 部分可繼續使用 static，加解密異動性不高。

4. 多語系需求

   1. 收銀頁 ( .net core 預設語系檔 )
   2. ApiResponse，~~Api 回應訊息 (一律英文)~~ ，內部可以中文，但 External Gateway 需要對應英文訊息

5. 情境模擬 (Respository 的 SP，如下 SQL 指令) 

   結論：建立個別情境不同的 SP，但最終結果是輸出 Entity ID，並利用此 ID 查詢建立 Domain Model。

```SQL
SELECT TOP 1 [Order].Id As Id, [order].OrderNo As OrderNo
FROM dbo. ThirdPartyOrders [Order WITH( NOLOCK
INNER JOIN dbo.ThirdPartyOrdersExtend [Extend] WITH(NOLOCK) ON [Extend].ThirdPartyorderId = [Order].Id
INNER JOIN dbo.MerchantOrders [MerchantOrder] WITH(NOLOCK ON [MerchantOrder].Id = [Order].MerchantorderId
WHERE [Order].ThirdPartyChannelid = @ThirdPartyChannelId
AND [Order].OrderStatus = 2 --Process 
AND [Order].Ordertime >= DATEADD(HOUR, -2, dbo.fn_GetSysDate())
AND [Extend].[Address] LIKE '%' @Address +'%' --討論此行
AND [Merchantorder].OrderStatus = 0  --Unprocessed
ORDER BY [Order].OrderTime DESC
```

---

# 基礎邏輯架構圖

![pictures/Untitled.png](D:\Jack's Home\個人\Code\SampleProject\file\pictures\Untitled.png)

邏輯組成

![pictures/Untitled%201.png](D:\Jack's Home\個人\Code\SampleProject\file\pictures\Untitled 1.png)

---

