using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.Domain.Shared
{
    public class Currency
    {
        public int Id { get; private set; }
        public string CurrencyCode { get; private set; } = null!;
        public string? Name { get; private set; }
        public DateTime? ModifiedDate { get; private set; }
    }
}
