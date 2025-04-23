using SampleProject.Domain.Queries.ViewModel;

namespace SampleProject.Domain.Interfaces.Query
{
    public interface IOrderQuery
    {
        Task<OrderViewModel> GetOrderAsync(int id);

        Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId);

        Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}