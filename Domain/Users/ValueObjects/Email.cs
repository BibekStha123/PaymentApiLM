using System.Text.RegularExpressions;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.User.ValueObjects
{
    public sealed partial class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required.", nameof(value));

            var normalized = value.Trim().ToLowerInvariant();

            if (!EmailRegex().IsMatch(normalized))
                throw new ArgumentException($"'{value}' is not a valid email address.", nameof(value));

            return new Email(normalized);
        }

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
