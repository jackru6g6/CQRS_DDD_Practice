using SampleProject.Domain.Domains.Aggregate.Order;

namespace SampleProject.Domain.Interfaces.Repository
{
    public interface IOrderAggRepository
    {
        OrderAgg Get(Guid id);

        void Add(OrderAgg order);

        void Update(OrderAgg order);

        void Delete(OrderAgg order);
    }
}