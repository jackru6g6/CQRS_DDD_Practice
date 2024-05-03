namespace SampleProject.Domain.Repositories.Entity
{
    public class OrderEntity
    {
        public Guid Id { get; set; }

        public string No { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}