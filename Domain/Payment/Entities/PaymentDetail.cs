using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;

namespace PaymentDetailApi.Domain.Payment.Entities
{
    public class PaymentDetail
    {
        public int Id { get; private set; }
        public string CardOwnerName { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpirationDate { get; private set; }
        public string SecurityCode { get; private set; }

        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

        public PaymentDetail(string cardOwnerName, string cardNumber, string expirationDate, string securityCode)
        {
            CardOwnerName = cardOwnerName;
            CardNumber = cardNumber;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;

            AddDomainEvent(new PaymentCreatedDomainEvent(this));
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearEvents() => _domainEvents.Clear();

    }
}
