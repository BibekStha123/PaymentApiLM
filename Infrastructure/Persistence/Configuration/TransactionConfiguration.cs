using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Domain.Shared.ValueObjects;
using PaymentDetailApi.Domain.Transactions.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();

            builder.Property(t => t.Amount)
                .HasConversion(money => money.Amount, value => Money.Create(value))
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.CreatedAt).HasColumnType("datetime2");

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PaymentDetail>()
                .WithMany()
                .HasForeignKey(t => t.PaymentDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Currency>()
                .WithMany()
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
