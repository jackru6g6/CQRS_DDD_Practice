---
name: nuget-pack-workflow
description: 'Use when packaging a NuGet package, creating a .nupkg, running dotnet pack, validating package metadata, or preparing a reusable .NET library for release. Trigger for NuGet pack, package build, csproj packability, version check, and package verification workflows.'
argument-hint: '目標專案名稱或 .csproj 路徑，例如 SampleProject.Domain 或 SampleProject.API.Model'
user-invocable: true
disable-model-invocation: false
---

# NuGet Package Workflow

## 這個 skill 會做什麼

這個 skill 用來處理目前 workspace 內的 NuGet 套件打包流程，特別適合 .NET 類別庫專案。它會引導代理依序完成：

1. 找出適合被打包的專案。
2. 檢查版本與套件 Metadata 是否足夠。
3. 執行 restore、build、必要測試與 dotnet pack。
4. 驗證產出的 `.nupkg` 是否存在、版本是否正確、輸出位置是否合理。
5. 在缺少 PackageId、Version、IsPackable 或必要描述資訊時，先停下來說明缺口，再決定是否補齊。

## 何時使用

- 使用者要求「打包 NuGet 套件」或「幫我出 `.nupkg`」。
- 使用者提到 `dotnet pack`、`NuGet`、`PackageId`、`Version`、`nupkg`、`IsPackable`。
- 要確認某個 `.csproj` 是否適合做成套件。
- 要在目前 repo 內針對類別庫專案做發佈前驗證，但不包含推送到 NuGet 或私有來源。

## 目前 repo 的預設判斷

- 優先檢查類別庫專案，而不是 API 或測試專案。
- 目前 workspace 中較可能被打包的專案包含：`SampleProject.Domain`、`SampleProject.API.Model`，以及 `DDDSampleProj` 下的 domain/model 類型專案。
- 若專案已有 `IsPackable=false`，不要強行打包。
- 若專案只有 `AssemblyVersion`/`FileVersion`，但沒有 `PackageId`、`Version`、`PackageVersion`，要先提醒使用者套件 Metadata 可能不足。

## 操作流程

### 1. 確認目標專案

先找出使用者要打包的專案或 `.csproj`。如果使用者只說「幫我打包套件」，就依下列原則過濾：

- 排除 Web API、可執行專案、測試專案。
- 優先選擇被其他專案參考的共用類別庫。
- 若有多個候選專案，先列出候選並請使用者確認。

### 2. 檢查打包前置條件

閱讀目標 `.csproj`，至少檢查：

- `TargetFramework` 或 `TargetFrameworks`
- `PackageId`
- `Version` 或 `PackageVersion`
- `IsPackable`
- `Authors`、`Description`、`PackageTags`、`RepositoryUrl` 是否有需要補齊

判斷規則：

- 若明確存在 `IsPackable=false`，停止並告知該專案目前被設定為不可打包。
- 若缺少 `PackageId`，預設可暫用專案名稱，但要明確提醒這是臨時推論。
- 若缺少 `Version`/`PackageVersion`，檢查是否只有 `AssemblyVersion`/`FileVersion`；若是，先沿用它們作為建議的套件版本，並明確告知這是從組件版本推導而來。
- 若缺少描述性 Metadata，不一定阻擋本地 pack，但要提示這會影響正式發佈品質。

若缺少常見 NuGet Metadata，可提供最小必要範本供使用者確認後再補：

```xml
<PropertyGroup>
	<PackageId>My.Package</PackageId>
	<Version>1.0.0</Version>
	<Authors>TeamName</Authors>
	<Description>Package description</Description>
	<PackageTags>tag1;tag2</PackageTags>
</PropertyGroup>
```

補欄位原則：

- `PackageId`、`Version` 屬於高優先欄位，會直接影響套件識別。
- `Authors`、`Description`、`PackageTags` 屬於建議欄位，影響套件可讀性與發佈品質。
- 未經使用者確認，不要擅自填入正式對外名稱或版本。

### 3. 確認依賴與建構範圍

在 pack 前，先確認：

- 專案參考的內部相依專案可成功 restore。
- 若該類別庫有關聯測試，優先執行與該專案直接相關的測試，而不是盲目跑完整 repo。
- 若 repo 有多個 solution，優先使用與目標專案相符的 solution 或直接對 `.csproj` 操作。

### 4. 執行打包

預設命令模式：

```powershell
dotnet restore <target.csproj>
dotnet build <target.csproj> -c Release
dotnet pack <target.csproj> -c Release --no-build -o .\artifacts\nuget
```

若需要明確覆蓋版本，可在使用者同意後使用：

```powershell
dotnet pack <target.csproj> -c Release --no-build -o .\artifacts\nuget /p:PackageVersion=<version>
```

若目標專案沒有 `Version` 或 `PackageVersion`，但有 `AssemblyVersion`/`FileVersion`，優先把該值當成建議輸入版本，再交由使用者確認是否要覆蓋。

執行原則：

- 除非有理由，優先用 `Release`。
- 若剛做過 `build`，pack 時用 `--no-build` 避免重複。
- 輸出先集中到 workspace 可辨識的資料夾，例如 `artifacts/nuget`。
- 若 repo 已有既定輸出規則，優先遵循既有結構。

### 5. 驗證結果

完成後至少檢查：

- `.nupkg` 是否成功產生。
- 套件名稱與版本是否符合預期。
- 輸出路徑是否正確。
- 若有 `.snupkg` 或 symbols 套件需求，明確說明目前是否未處理。

可額外做的驗證：

- 用解壓或 NuGet 檢視方式確認 package 內容。
- 檢查 dependency graph 是否合理。
- 若使用者要求，可補做本地來源安裝驗證，但這不屬於預設流程。

## 分支處理

### 找不到明確可打包專案

- 列出候選類別庫專案。
- 說明為何 API/測試專案不應納入。
- 要求使用者指定目標。

### csproj 缺少 NuGet 必要欄位

- 先指出缺少哪些欄位。
- 說明哪些欄位只影響套件品質，哪些欄位會直接影響版本或識別。
- 若使用者同意，再補 `.csproj` 後重新 pack。
- 若專案已有 `AssemblyVersion`/`FileVersion`，可以先把它當成建議值，補進 `Version` 或在 pack 時用 `/p:PackageVersion=` 明確指定。

### 打包失敗

依序判斷：

1. restore 失敗：先看套件來源或內部相依是否缺失。
2. build 失敗：先修建構錯誤，不要跳過。
3. pack 失敗：檢查 `IsPackable`、版本屬性、輸出路徑、重複產物、target framework 設定。

### 只想驗證，不想修改專案

- 可以只做檢查與嘗試 pack。
- 若缺欄位造成風險，明確說明但不要擅自補寫 Metadata。

## 完成標準

符合以下條件才算完成：

- 已確認正確的目標專案。
- 已執行或明確評估 restore、build、pack。
- 已回報 `.nupkg` 產出位置。
- 已說明版本來源與任何缺漏的 package metadata。
- 若未完成，也要明確指出卡在哪一步以及下一個最小動作。

## 互動範例

- `/nuget-pack-workflow SampleProject.Domain`
- `幫我打包 SampleProject.API.Model 成 NuGet 套件`
- `檢查這個 csproj 能不能做 dotnet pack`
- `幫我驗證這次 NuGet 打包會不會缺少版本資訊`

## 執行時的工作原則

- 先讀 `.csproj` 再執行命令，不要先盲目 pack。
- 若 repo 有多個相似專案，先消除目標歧義。
- 沒有使用者同意時，不要把流程延伸到推送或發版。
- 修改 `.csproj` 時只補齊與打包直接相關的最小欄位。