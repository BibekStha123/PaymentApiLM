namespace PaymentDetailApi.Application.PaymentDetail
{
    public record PaymentDetailResponse(
        Guid Id,
        string CardOwnerName,
        string CardNumber,
        string ExpirationDate,
        string SecurityCode,
        bool Active
    );
}
