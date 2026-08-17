using System.Text.RegularExpressions;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Payment.ValueObjects
{
    public sealed partial class ExpirationDate : ValueObject
    {
        public string Value { get; }
        public int Month { get; }
        public int Year { get; }

        private ExpirationDate(string value, int month, int year)
        {
            Value = value;
            Month = month;
            Year = year;
        }

        public static ExpirationDate Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !ExpirationDateRegex().IsMatch(value))
                throw new ArgumentException("Expiration date must be in MM/YY format.", nameof(value));

            var parts = value.Split('/');
            var month = int.Parse(parts[0]);
            var year = 2000 + int.Parse(parts[1]);

            return new ExpirationDate(value, month, year);
        }

        public bool IsExpired
        {
            get
            {
                var firstDayOfNextMonth = new DateTime(Year, Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
                return DateTime.UtcNow >= firstDayOfNextMonth;
            }
        }

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        [GeneratedRegex(@"^(0[1-9]|1[0-2])\/\d{2}$")]
        private static partial Regex ExpirationDateRegex();
    }
}
