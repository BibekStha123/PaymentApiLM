namespace PaymentDetailApi.Application.Currency
{
    public record CurrencyResponse(
        int Id,
        string CurrencyCode,
        string Name
    );
}
