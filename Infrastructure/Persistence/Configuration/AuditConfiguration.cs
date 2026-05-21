using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Infrastructure.Persistence.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class AuditConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.Details)
                .HasColumnType("nvarchar(200)");

            builder.HasOne(x => x.Payment)
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
