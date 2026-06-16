using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Payments
{
    public class PaymentDeletedAuditHandler : IDomainEventHandler<PaymentDeletedDomainEvent>
    {
        private readonly PaymentDetailsContext _context;
        public PaymentDeletedAuditHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public Task Handle(PaymentDeletedDomainEvent domainEvent)
        {
            _context.AuditLogs.Add(new Persistence.Entities.AuditLog
            {
                Action = "Delete",
                PaymentId = domainEvent.PaymentDetails.Id,
                Details = $"Payment deleted for {domainEvent.PaymentDetails.Id}"
            });

            return Task.CompletedTask;
        }
    }
}
