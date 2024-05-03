namespace SampleProject.Domain.Interfaces.Domain.Validation
{
    public interface IValidator<in TRequest>
    {
        /// <summary>
        /// 驗證
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<string> Validate(TRequest request);
    }
}