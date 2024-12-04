using MediatR;

namespace SampleProject.Domain.Domains.Command.Order
{
    public record OrderCreatedCommand : IRequest<Guid>
    {
        public string Name { get; init; }

        public decimal Amount { get; init; }
    }
}