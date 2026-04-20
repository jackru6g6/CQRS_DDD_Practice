namespace SampleProject.Domain.Interceptors.OptimisticLock.Attribute
{
    /// <summary>
    /// 標記為新增操作，供攔截器識別
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AddAttribute : RepositoryAttribute { }
}
