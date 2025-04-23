namespace SampleProject.Domain.Interfaces.Repository
{
    /// <summary>
    /// 歡樂鎖定義
    /// </summary>
    /// <remarks>
    /// 基本綁定在 RootEntity 上，查看當前 Entity 是否被重複動作(新增、編輯、刪除等)
    /// </remarks>
    public interface IOptimisticLock
    {
        /// <summary>
        /// 金鑰
        /// </summary>
        /// <remarks>
        /// 大多數會是 PK 欄位
        /// </remarks>
        string Key { get; }

        /// <summary>
        /// 版本
        /// </summary>
        /// <remarks>
        /// 大多欄位會是"編輯時間"欄位初始值，再進行DB交互時可以比對當前值是否跟DB一致，一致則正常，不一致則需要重新邏輯
        /// </remarks>
        string? Version { get; }
    }
}