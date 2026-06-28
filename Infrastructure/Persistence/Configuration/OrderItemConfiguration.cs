using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Orders.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.Property(i => i.OrderId).IsRequired();
            builder.Property(i => i.UnitPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(i => i.Quantity).IsRequired();

            builder.Ignore(i => i.TotalPrice);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
