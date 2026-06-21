namespace PaymentDetailApi.Application.Products
{
    public sealed record ProductResponse(
        int Id,
        string Name,
        string Description,
        decimal Price,
        int Stock,
        string CategoryName);
}
