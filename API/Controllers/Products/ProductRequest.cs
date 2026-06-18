namespace PaymentDetailApi.API.Controllers.Products
{
    public sealed record ProductRequest(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        int CategoryId,
        bool IsActive);
}
