namespace PaymentDetailApi.Application.Currency
{
    public record CurrencyResponse(
        Guid Id,
        string CurrencyCode,
        string Name
    );
}
