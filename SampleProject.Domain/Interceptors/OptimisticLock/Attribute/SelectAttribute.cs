namespace SampleProject.Domain.Interceptors.OptimisticLock.Attribute
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SelectAttribute : System.Attribute { }
}