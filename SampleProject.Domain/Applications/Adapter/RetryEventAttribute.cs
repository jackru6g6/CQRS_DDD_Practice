namespace SampleProject.Domain.Applications.Adapter
{
    /// <summary>
    /// 可以重複執行的 Event
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RetryEventAttribute : Attribute { }
}
