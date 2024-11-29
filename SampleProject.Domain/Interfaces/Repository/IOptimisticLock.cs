namespace SampleProject.Domain.Interfaces.Repository
{
    public interface IOptimisticLock
    {
        string Key { get; }

        string? Version { get; }
    }
}