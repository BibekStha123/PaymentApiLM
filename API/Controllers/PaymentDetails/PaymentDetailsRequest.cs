namespace PaymentDetailApi.API.Controllers.PaymentDetails
{
    public sealed record PaymentDetailRequest(
        string CardNumber,
        string ExpirationDate,
        string SecurityCode);
}
