namespace SampleProject.Domain.Interceptors.OptimisticLock.Attribute
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RepositoryAttribute : System.Attribute { }
}