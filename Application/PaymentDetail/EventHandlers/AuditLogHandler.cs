using MediatR;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;
using PaymentDetailApi.Infrastructure;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Application.PaymentDetail.EventHandlers
{
    public class AuditLogHandler : INotificationHandler<PaymentCreatedDomainEvent>
    {
        private readonly PaymentDetailsContext _context;
        public AuditLogHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public Task Handle(PaymentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "Create",
                PaymentId = notification.PaymentDetails.Id,
                Details = $"Payment created for {notification.PaymentDetails.CardOwnerName}"
            });

            return Task.CompletedTask;
        }
    }
}
