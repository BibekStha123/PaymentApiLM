namespace PaymentDetailApi.Application.PaymentDetail
{
    public record PaymentDetailResponse(
        int Id,
        string CardOwnerName,
        string CardNumber,
        string ExpirationDate,
        string SecurityCode,
        bool Active
    );
}
