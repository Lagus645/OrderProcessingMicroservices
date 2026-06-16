using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.DataAccess.Postgres.Models;

namespace PaymentService.DataAccess.Postgres.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
