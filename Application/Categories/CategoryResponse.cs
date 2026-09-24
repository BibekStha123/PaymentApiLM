namespace PaymentDetailApi.Application.Categories
{
    public sealed record CategoryResponse(
        Guid Id,
        string Name,
        string Type);
}
