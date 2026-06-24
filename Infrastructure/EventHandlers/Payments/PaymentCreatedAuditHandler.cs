using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Payments
{
    public class PaymentCreatedAuditHandler : IDomainEventHandler<PaymentCreatedDomainEvent>
    {
        private readonly PaymentDetailsContext _context;
        public PaymentCreatedAuditHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public Task Handle(PaymentCreatedDomainEvent domainEvent)
        {
            _context.Logs.Add(Log.Create(
                "Create",
                "Payment",
                domainEvent.PaymentDetails.Id,
                $"Payment created for user {domainEvent.PaymentDetails.UserId}"
            ));

            return Task.CompletedTask;
        }
    }
}
