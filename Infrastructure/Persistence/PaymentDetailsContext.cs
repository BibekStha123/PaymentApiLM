using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Domain.User.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence
{
    public class PaymentDetailsContext : DbContext
    {
        public PaymentDetailsContext(DbContextOptions<PaymentDetailsContext> options)
            : base(options)
        {
        }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDetailsContext).Assembly);
        }
    }

    public class PaymentDetailsContextFactory : IDesignTimeDbContextFactory<PaymentDetailsContext>
    {
        public PaymentDetailsContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "API"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PaymentDetailsContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("PaymentDetailContext")
                ?? throw new InvalidOperationException("Connection string 'PaymentDetailContext' not found."));

            return new PaymentDetailsContext(optionsBuilder.Options);
        }
    }
}
