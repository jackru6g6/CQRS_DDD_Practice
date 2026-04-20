using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleProject.Domain.Repositories.Entity;

namespace SampleProject.Domain.Infrastructures.EntityConfigurations
{
    /// <summary>
    /// OrderEntity EF Core 欄位設定
    /// </summary>
    public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .IsRequired();

            builder.Property(t => t.No)
                   .HasMaxLength(50);

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.CreateTime)
                   .IsRequired();

            builder.Property(t => t.ModifyTime);

            // 忽略 Version（由 Lazy 計算而來，不對應欄位）
            builder.Ignore(t => t.Version);
            builder.Ignore(t => t.Key);
        }
    }
}
