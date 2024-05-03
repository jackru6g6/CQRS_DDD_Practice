namespace SampleProject.API.Model.Base
{
    /// <summary>
    /// ApiResult
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 返回代碼
        /// </summary>
        //[JsonProperty("code", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        /// <summary>
        /// 返回訊息
        /// </summary>
        //[JsonProperty("message", Order = 3)]
        public string Message { get; set; }

        /// <summary>
        /// 判斷Code是否成功
        /// </summary>
        //[JsonIgnore]
        public bool IsSucceed => Code == ApiResponse.Success.Code &&
                                 Message == ApiResponse.Success.Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ApiResult(string code = "", string message = "")
        {
            Code = code;
            Message = message;
        }
    }

    /// <summary>
	/// Outer Controllers API Use
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ApiResult<T> : ApiResult where T : class
    {
        /// <summary>
        /// 額外資訊
        /// </summary>
        //[JsonProperty("data", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        public ApiResult(string code = "", string message = "", T data = null)
            : base(code, message)
        {
            Data = data;
        }
    }
}
