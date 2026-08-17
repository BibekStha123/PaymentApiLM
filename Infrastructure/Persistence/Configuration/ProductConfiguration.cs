using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Shared.ValueObjects;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
                .HasConversion(money => money.Amount, value => Money.Create(value))
                .HasPrecision(18, 2);

            builder.Property(p => p.Stock)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
