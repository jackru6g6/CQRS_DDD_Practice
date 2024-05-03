using MediatR;
using Microsoft.Extensions.Logging;
using SampleProject.Domain.Interfaces.Domain.Validation;

namespace SampleProject.Domain.Applications.Behavior
{
    /// <summary>
    /// 驗證器，每個命令的驗證
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorBehavior(ILogger<ValidatorBehavior<TRequest, TResponse>> logger, IEnumerable<IValidator<TRequest>> validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = GetGenericTypeName(request);

            var failures = _validators
               .Select(v => v.Validate(request).ToList())
               .SelectMany(t => t)
               .ToList();

            if (failures.Any())
            {
                //throw new Exception(ApiResponse.IncorrectParameters,
                //       responseMessage: $"验证失败。{string.Join('、', failures)}。",
                //       logMessage: $"Validating command {GetGenericTypeName(request)}.error message:{string.Join('、', failures)}");
            }

            return await next();
        }

        private string GetGenericTypeName(Type type)
        {
            string typeName;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        private string GetGenericTypeName(object @object) => GetGenericTypeName(@object.GetType());
    }
}
