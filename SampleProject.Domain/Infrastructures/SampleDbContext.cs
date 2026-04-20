using Microsoft.EntityFrameworkCore;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Infrastructures
{
    /// <summary>
    /// EF Core DbContext
    /// </summary>
    public class SampleDbContext : DbContext
    {
        /// <summary>
        /// 訂單資料表
        /// </summary>
        public DbSet<OrderEntity> Orders { get; set; }

        /// <summary>
        /// 訂單明細資料表
        /// </summary>
        public DbSet<OrderItemEntity> OrderItems { get; set; }

        /// <summary>
        /// 訂單歷程資料表
        /// </summary>
        public DbSet<OrderEventEntity> OrderEvents { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
        }
    }
}
