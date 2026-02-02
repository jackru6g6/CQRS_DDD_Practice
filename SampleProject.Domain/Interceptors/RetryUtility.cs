namespace SampleProject.Domain.Interceptors
{
    public static class RetryUtility
    {
        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 嘗試執行一個非同步操作，如果遇到指定的例外，則進行重試。
        /// </summary>
        /// <param name="operation">要執行的非同步操作。</param>
        /// <typeparam name="TResult">操作的返回類型。</typeparam>
        /// <returns>操作成功後的結果。</returns>
        public static async Task<TResult> ExecuteWithRetry<TResult>(Func<Task<TResult>> operation)
        {
            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    // 嘗試執行操作
                    return await operation();
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is System.TimeoutException)
                {
                    // 這是我們想要重試的暫時性例外 (Transient Fault)
                    if (attempt < MaxRetryAttempts)
                    {
                        //System.Console.WriteLine($"[重試機制] 第 {attempt} 次失敗。等待 {Delay.TotalSeconds} 秒後重試...");

                        await Task.Delay(Delay);

                        continue;
                    }

                    // 達到最大重試次數，拋出原始例外
                    //System.Console.WriteLine($"[重試機制] 達到最大重試次數 ({MaxRetryAttempts})，終止。");

                    throw;
                }
            }

            // 為了編譯器，這行理論上不應被執行
            throw new InvalidOperationException("Retry loop completed unexpectedly.");
        }

        /// <summary>
        /// 嘗試執行一個非同步操作，如果遇到指定的例外，則進行重試。
        /// </summary>
        /// <param name="operation">要執行的非同步操作。</param>
        /// <typeparam name="TResult">操作的返回類型。</typeparam>
        /// <returns>操作成功後的結果。</returns>
        public static async Task ExecuteWithRetryTask(Func<Task> operation)
        {
            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    // 嘗試執行操作
                    await operation();

                    return;
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is System.TimeoutException)
                {
                    // 這是我們想要重試的暫時性例外 (Transient Fault)
                    if (attempt < MaxRetryAttempts)
                    {
                        //System.Console.WriteLine($"[重試機制] 第 {attempt} 次失敗。等待 {Delay.TotalSeconds} 秒後重試...");

                        await Task.Delay(Delay);

                        continue;
                    }

                    // 達到最大重試次數，拋出原始例外
                    //System.Console.WriteLine($"[重試機制] 達到最大重試次數 ({MaxRetryAttempts})，終止。");

                    throw;
                }
            }

            // 為了編譯器，這行理論上不應被執行
            throw new InvalidOperationException("Retry loop completed unexpectedly.");
        }
    }
}
