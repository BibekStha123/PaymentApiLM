using MediatR;
using PaymentDetailApi.Features.PaymentDetail.Events;
using PaymentDetailApi.Infrastructure;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Features.PaymentDetail.EventHandlers
{
    public class AuditLogHandler : INotificationHandler<PaymentCreatedEvent>
    {
        private readonly PaymentDetailsContext _context;
        public AuditLogHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
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
