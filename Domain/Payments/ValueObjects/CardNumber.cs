using System.Text.RegularExpressions;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Payment.ValueObjects
{
    public sealed partial class CardNumber : ValueObject
    {
        public string Value { get; }

        private CardNumber(string value)
        {
            Value = value;
        }

        public static CardNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !CardNumberRegex().IsMatch(value))
                throw new ArgumentException("Card number must be exactly 16 digits.", nameof(value));

            return new CardNumber(value);
        }

        public string Masked() => $"**** **** **** {Value[^4..]}";

        public override string ToString() => Masked();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        [GeneratedRegex(@"^\d{16}$")]
        private static partial Regex CardNumberRegex();
    }
}
