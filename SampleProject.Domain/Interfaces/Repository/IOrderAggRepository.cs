using SampleProject.Domain.Domains.Aggregate.Order;

namespace SampleProject.Domain.Interfaces.Repository
{
    public interface IOrderAggRepository
    {
        Order Get(Guid id);

        void Add(Order order);

        void Update(Order order);

        void Delete(Order order);
    }
}