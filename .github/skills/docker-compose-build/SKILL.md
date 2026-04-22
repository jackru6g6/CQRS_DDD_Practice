---
name: docker-compose-build
description: 'Use when building or starting Docker Compose. Automatically runs dotnet build first, then docker-compose up -d. Reports errors immediately if any step fails. Trigger phrases: 建置 docker compose、啟動 docker compose、跑 docker compose、docker compose 建置。'
argument-hint: '可選：指定特定服務名稱，例如 web'
---

# Docker Compose 建置工作流程

## 適用時機
- 使用者說「建置 docker compose」
- 使用者說「啟動 docker compose」
- 使用者說「跑 docker compose」
- 需要重新建置並啟動 Docker Compose 環境

## 執行流程

### 步驟 1：執行 dotnet build
在終端機執行以下指令，建置整個方案：
```bash
dotnet build
```
- 若建置**失敗**：立即回報錯誤訊息，**停止後續步驟**，不得繼續執行 `docker-compose up`。
- 若建置**成功**：繼續步驟 2。

### 步驟 2：執行 docker-compose up
```bash
docker-compose up -d
```
- 若啟動**失敗**：立即回報錯誤訊息與對應服務名稱。
- 若啟動**成功**：回報各服務的存取網址。

## 成功後回報資訊
| 服務 | 網址 |
|------|------|
| Web API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| RabbitMQ 管理介面 | http://localhost:15672 |
| MSSQL | localhost:1433 |

## 錯誤處理原則
- 任何步驟失敗時，**立即停止**並回報完整錯誤訊息。
- 不要跳過錯誤繼續執行後續步驟。
- 提供可能的解決建議。
