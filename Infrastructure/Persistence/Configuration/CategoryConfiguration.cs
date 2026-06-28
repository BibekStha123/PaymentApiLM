using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.Catalog.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.Type)
                .HasColumnType("nvarchar(50)");
        }
    }
}
