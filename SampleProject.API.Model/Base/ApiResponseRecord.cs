namespace SampleProject.API.Model.Base
{
    /// <summary>
    /// 定義 api 回應內容
    /// </summary>
    public record ApiResponseRecord
    {
        /// <summary>
        /// 代碼
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }

        public ApiResponseRecord(string code, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
