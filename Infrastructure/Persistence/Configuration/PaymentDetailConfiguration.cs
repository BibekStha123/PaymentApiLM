using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Payment.Entities;
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

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
