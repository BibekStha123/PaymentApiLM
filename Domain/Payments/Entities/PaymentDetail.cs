using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;
using PaymentDetailApi.Domain.Payment.ValueObjects;

namespace PaymentDetailApi.Domain.Payment.Entities
{
    public class PaymentDetail : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public CardNumber CardNumber { get; private set; } = null!;
        public ExpirationDate ExpirationDate { get; private set; } = null!;
        public string SecurityCode { get; private set; } = null!;
        public bool Active { get; private set; }
        private PaymentDetail() { } // for EF Core materialization

        public PaymentDetail(Guid userId, CardNumber cardNumber, ExpirationDate expirationDate, string securityCode)
        {
            Validate(userId, expirationDate, securityCode);

            UserId = userId;
            CardNumber = cardNumber;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;
            Active = true;

            AddDomainEvent(new PaymentCreatedDomainEvent(this));
        }

        public void Delete()
        {
            Active = false;
            AddDomainEvent(new PaymentDeletedDomainEvent(this));
        }

        private static void Validate(Guid userId, ExpirationDate expirationDate, string securityCode)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.", nameof(userId));
            if (expirationDate.IsExpired)
                throw new ArgumentException("Card has already expired.", nameof(expirationDate));

            if (string.IsNullOrWhiteSpace(securityCode) || !System.Text.RegularExpressions.Regex.IsMatch(securityCode, @"^\d{3,4}$"))
                throw new ArgumentException("Security code must be 3 or 4 digits.", nameof(securityCode));
        }
    }
}
