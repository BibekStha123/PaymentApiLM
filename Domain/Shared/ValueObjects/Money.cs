using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Shared.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }

        private Money(decimal amount)
        {
            Amount = amount;
        }
        public static Money Zero { get; } = new(0m);

        public static Money Create(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            return new Money(amount);
        }

        public Money Add(Money other)
        {
            ArgumentNullException.ThrowIfNull(other);
            return new Money(Amount + other.Amount);
        }

        public Money Multiply(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            return new Money(Amount * quantity);
        }

        public static Money operator +(Money left, Money right) => left.Add(right);

        public static Money operator *(Money money, int quantity) => money.Multiply(quantity);

        public override string ToString() => Amount.ToString("0.00");

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
        }
    }
}
