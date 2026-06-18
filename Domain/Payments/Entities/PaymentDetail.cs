using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Payment.Events;

namespace PaymentDetailApi.Domain.Payment.Entities
{
    public class PaymentDetail : Entity
    {
        public int Id { get; private set; }
        public string CardOwnerName { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpirationDate { get; private set; }
        public string SecurityCode { get; private set; }
        public bool Active { get; private set; }
        private PaymentDetail() { } // for EF Core materialization

        public PaymentDetail(string cardOwnerName, string cardNumber, string expirationDate, string securityCode)
        {
            Validate(cardOwnerName, cardNumber, expirationDate, securityCode);

            CardOwnerName = cardOwnerName;
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

        private static void Validate(string cardOwnerName, string cardNumber, string expirationDate, string securityCode)
        {
            if (string.IsNullOrWhiteSpace(cardOwnerName))
                throw new ArgumentException("Card owner name is required.", nameof(cardOwnerName));

            if (string.IsNullOrWhiteSpace(cardNumber) || !System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^\d{16}$"))
                throw new ArgumentException("Card number must be exactly 16 digits.", nameof(cardNumber));

            if (string.IsNullOrWhiteSpace(expirationDate) || !System.Text.RegularExpressions.Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                throw new ArgumentException("Expiration date must be in MM/YY format.", nameof(expirationDate));

            if (string.IsNullOrWhiteSpace(securityCode) || !System.Text.RegularExpressions.Regex.IsMatch(securityCode, @"^\d{3,4}$"))
                throw new ArgumentException("Security code must be 3 or 4 digits.", nameof(securityCode));
        }
    }
}
