namespace SampleProject.API.Model.Order.Response
{
    public class GetOrderResponse
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public decimal? Amount { get; set; }
    }
}
