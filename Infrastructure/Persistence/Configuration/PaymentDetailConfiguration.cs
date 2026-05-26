using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class PaymentDetailConfiguration : IEntityTypeConfiguration<PaymentDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentDetail> builder)
        {
            builder.ToTable("PaymentDetails", t => t.HasTrigger("[trg_AfterPaymentDetailsInsert]"));
        }
    }
}
