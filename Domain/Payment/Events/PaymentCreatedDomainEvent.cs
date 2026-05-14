using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Domain.Payment.Events
{
    public class PaymentCreatedDomainEvent : DomainEvent
    {
        public PaymentDetail PaymentDetails { get; }
        public PaymentCreatedDomainEvent(PaymentDetail paymentDetails)
        {
            PaymentDetails = paymentDetails;
        }
    }
}
