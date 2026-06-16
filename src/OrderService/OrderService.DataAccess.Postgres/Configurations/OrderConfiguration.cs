using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.DataAccess.Postgres.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.EmailClient)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
