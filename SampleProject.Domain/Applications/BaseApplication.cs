using SampleProject.API.Model.Base;
using SampleProject.Domain.Infrastructures;

namespace SampleProject.Domain.Applications
{
    public class BaseApplication
    {
        public BaseApplication() { }

        /// <summary>
        /// 成功回應處理
        /// </summary>
        protected ApiResult HandleSuccess()
            => new(code: ApiResponse.Success.Code,
                   message: ApiResponse.Success.Message);

        /// <summary>
        /// 成功回應處理(有回傳值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        protected ApiResult<T> HandleSuccess<T>(object data) where T : class
            => new(code: ApiResponse.Success.Code,
                   message: ApiResponse.Success.Message,
                   data: MapperData<T>(data));

        protected ApiResult<T> HandleFail<T>(ApiResponseRecord errorResponse, object data) where T : class
            => new(code: errorResponse.Code,
                   message: errorResponse.Message,
                   data: MapperData<T>(data));

        private T? MapperData<T>(object data) => data switch
        {
            // 空值，回傳null
            null => default,
            // 空陣列，回傳null
            IEnumerable<object> datas when !datas.Any() => default,
            // 自己，不用特意轉型
            T result => result,
            // 其餘，都轉型
            _ => MapperProvider.Map<T>(data),
        };
    }
}