using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentDetailApi.Domain.User.Entities;
using PaymentDetailApi.Domain.User.ValueObjects;

namespace PaymentDetailApi.Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedNever();
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email)
                .HasConversion(email => email.Value, value => Email.Create(value))
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);
            builder.Property(u => u.DisplayName).HasMaxLength(100);
            builder.Property(u => u.CreatedAt).HasColumnType("datetime2");
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.UserName).IsUnique();
        }
    }
}
