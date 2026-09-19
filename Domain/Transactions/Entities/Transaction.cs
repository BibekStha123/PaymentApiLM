using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared.ValueObjects;

namespace PaymentDetailApi.Domain.Transactions.Entities
{
    public class Transaction : AggregateRoot
    {
        public Guid OrderId { get; private set; }
        public Guid PaymentDetailId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public Guid CurrencyId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Transaction() { } // for EF Core materialization

        private Transaction(Guid orderId, Guid paymentDetailId, Money amount, Guid currencyId)
        {
            Validate(orderId, paymentDetailId, currencyId);
            OrderId = orderId;
            PaymentDetailId = paymentDetailId;
            Amount = amount;
            CurrencyId = currencyId;
            CreatedAt = DateTime.UtcNow;
        }

        public static Transaction Create(Guid orderId, Guid paymentDetailId, Money amount, Guid currencyId)
        {
            return new Transaction(orderId, paymentDetailId, amount, currencyId);
        }

        private static void Validate(Guid orderId, Guid paymentDetailId, Guid currencyId)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("A valid order is required.", nameof(orderId));

            if (paymentDetailId == Guid.Empty)
                throw new ArgumentException("A valid payment detail is required.", nameof(paymentDetailId));

            if (currencyId == Guid.Empty)
                throw new ArgumentException("A valid currency is required.", nameof(currencyId));
        }
    }
}
