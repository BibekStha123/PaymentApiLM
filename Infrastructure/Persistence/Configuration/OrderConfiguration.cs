using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Orders;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Domain.User.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(o => o.OrderDate).HasColumnType("datetime2");

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion(
                    s => s.ToString(),
                    s => (OrderStatus)Enum.Parse(typeof(OrderStatus), s))
                .HasMaxLength(20);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Currency>()
                .WithMany()
                .HasForeignKey(o => o.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(o => o.TotalAmount);

            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
