using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;

namespace PaymentDetailApi.Domain.Payment.Entities
{
    public class PaymentDetail : AggregateRoot
    {
        public int Id { get; private set; }
        public string CardOwnerName { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpirationDate { get; private set; }
        public string SecurityCode { get; private set; }

        private PaymentDetail() { } // for EF Core materialization

        public PaymentDetail(string cardOwnerName, string cardNumber, string expirationDate, string securityCode)
        {
            CardOwnerName = cardOwnerName;
            CardNumber = cardNumber;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;

            AddDomainEvent(new PaymentCreatedDomainEvent(this));
        }
    }
}
