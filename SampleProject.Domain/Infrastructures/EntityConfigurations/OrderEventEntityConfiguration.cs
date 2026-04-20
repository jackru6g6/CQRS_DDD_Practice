using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Infrastructures.EntityConfigurations
{
    /// <summary>
    /// OrderEventEntity EF Core 欄位設定
    /// </summary>
    public class OrderEventEntityConfiguration : IEntityTypeConfiguration<OrderEventEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEventEntity> builder)
        {
            builder.ToTable("OrderEvents");

            builder.HasKey(t => t.OrderId);

            builder.Property(t => t.OrderId)
                   .IsRequired();

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.CreateTime)
                   .IsRequired();
        }
    }
}
