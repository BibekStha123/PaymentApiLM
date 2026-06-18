using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Catalog.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
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
