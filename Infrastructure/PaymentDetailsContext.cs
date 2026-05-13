using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Infrastructure
{
    public class PaymentDetailsContext : DbContext
    {
        public PaymentDetailsContext(DbContextOptions<PaymentDetailsContext> options)
            : base(options)
        {
        }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
