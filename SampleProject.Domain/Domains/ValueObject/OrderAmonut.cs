
using SampleProject.Domain.Enums;

namespace SampleProject.Domain.Domains.ValueObject
{
    public record OrderAmonut : ValueObjectRecord
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        public OrderAmonut(decimal amount, string currencyStr)
        {
            if (amount < 0)
            {
                throw new Exception($"{nameof(OrderAmonut)} amount is invalid.");
            }

            Amount = amount;
            Currency = Currency.FromName(currencyStr);
        }
    }
}
