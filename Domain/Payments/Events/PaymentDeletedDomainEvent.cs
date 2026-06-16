using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Domain.Payment.Events
{
    public class PaymentDeletedDomainEvent : DomainEvent
    {
        public PaymentDetail PaymentDetails { get; }
        public PaymentDeletedDomainEvent(PaymentDetail paymentDetails)
        {
            PaymentDetails = paymentDetails;
        }
    }
}
