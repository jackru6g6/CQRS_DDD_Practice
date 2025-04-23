namespace SampleProject.Domain.Exceptions
{
    public class EntityNullException : Exception
    {
        public EntityNullException(string entityName) : base($"{entityName} is null.") { }
    }
}