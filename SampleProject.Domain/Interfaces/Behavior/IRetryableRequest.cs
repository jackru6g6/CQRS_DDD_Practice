namespace SampleProject.Domain.Interfaces.Behavior
{
    /// <summary>
    /// 可重複執行的請求
    /// </summary>
    public interface IRetryableRequest
    {
        // 如果需要，可以定義屬性來獲取重試參數
        //int MaxAttempts { get; }
    }
}
