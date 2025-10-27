namespace SampleProject.Domain.Interfaces.Behavior
{
    /// <summary>
    /// 實作此介面的 Command 會使用 RedLockPipelineBehavior
    /// </summary>
    public interface IRequireRedLock
    {
        /// <summary>
        /// redlock key
        /// </summary>
        string LockKey { get; }
    }
}
