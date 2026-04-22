# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 複製解決方案與專案檔（利用 Docker layer cache 優化 restore 速度）
COPY SampleProject.sln ./
COPY SampleProject/SampleProject.API.csproj                       SampleProject/
COPY SampleProject.API.Model/SampleProject.API.Model.csproj       SampleProject.API.Model/
COPY SampleProject.Domain/SampleProject.Domain.csproj             SampleProject.Domain/
COPY SampleProject.NUnitTest/SampleProject.NUnitTest.csproj       SampleProject.NUnitTest/

RUN dotnet restore

# 複製所有原始碼
COPY . .

# 建構 & 發佈
RUN dotnet publish SampleProject/SampleProject.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# 建立非 root 使用者，提升容器安全性
# RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
# USER appuser

COPY --from=build /app/publish .

EXPOSE 80

# 設定容器啟動命令
ENTRYPOINT ["dotnet", "SampleProject.API.dll"]
