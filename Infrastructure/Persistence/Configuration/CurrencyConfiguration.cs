using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Shared;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CurrencyCode)
                .HasColumnType("nvarchar(3)");

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.ModifiedDate)
                .HasColumnType("datetime");
        }
    }
}
