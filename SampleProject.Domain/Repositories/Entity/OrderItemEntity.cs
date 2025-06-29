namespace SampleProject.Domain.Repositories.Entity
{
    public class OrderItemEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; }
    }
}
