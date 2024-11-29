using SampleProject.Domain.Interfaces.Repository;

namespace SampleProject.Domain.Repositories.Entity
{
    public class OrderEntity : IOptimisticLock
    {
        public string Key => Id.ToString();

        public string? Version => _version.Value;
        private readonly Lazy<string?> _version;

        public OrderEntity()
        {
            _version = new Lazy<string>(() => ModifyTime?.ToString());
        }

        public Guid Id { get; set; }

        public string No { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}