using SampleProject.Domain.Domains.Command.Order;
using SampleProject.Domain.Interfaces.Domain.Validation;

namespace SampleProject.Domain.Domains.CommandValidation.Order
{
    public class OrderCreatedCommandValidator : IValidator<OrderCreatedCommand>
    {
        public IEnumerable<string> Validate(OrderCreatedCommand request)
        {
            throw new NotImplementedException();
        }
    }
}
