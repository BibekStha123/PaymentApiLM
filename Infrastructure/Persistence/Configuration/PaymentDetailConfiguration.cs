using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Domain.Payment.ValueObjects;
using PaymentDetailApi.Domain.User.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class PaymentDetailConfiguration : IEntityTypeConfiguration<PaymentDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentDetail> builder)
        {
            builder.ToTable("PaymentDetails", t => t.HasTrigger("[trg_AfterPaymentDetailsInsert]"));

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.UserId).IsRequired();

            builder.Property(p => p.CardNumber)
                .HasConversion(cardNumber => cardNumber.Value, value => CardNumber.Create(value))
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(p => p.ExpirationDate)
                .HasConversion(expirationDate => expirationDate.Value, value => ExpirationDate.Create(value))
                .IsRequired()
                .HasMaxLength(5);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
