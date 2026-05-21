
using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Infrastructure.Persistence.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public PaymentDetail? Payment { get; set; }
    }
}
