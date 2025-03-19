namespace SampleProject.Domain.Extensions
{
    public static class MethodExtension
    {
        /// <summary>
        /// 安全執行指定的操作，捕捉例外並在失敗時返回 null。
        /// </summary>
        /// <typeparam name="T">返回值的類型。</typeparam>
        /// <param name="func">需要執行的操作（返回值的函數）。</param>
        /// <returns>執行成功時返回結果，失敗時返回 null。</returns>
        public static T? ExecuteSafely<T>(this Func<T> func) where T : class
        {
            try
            {
                return func();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}