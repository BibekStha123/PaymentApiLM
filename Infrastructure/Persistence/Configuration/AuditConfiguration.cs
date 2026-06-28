using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Shared;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class AuditConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Action)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.EntityName)
                .HasColumnType("nvarchar(100)");

            builder.Property(x => x.Details)
                .HasColumnType("nvarchar(200)");
        }
    }
}
