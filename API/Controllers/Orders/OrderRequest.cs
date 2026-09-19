namespace PaymentDetailApi.API.Controllers.Orders
{
    public sealed record OrderRequest(
        string ShippingAddress,
        Guid CurrencyId,
        Guid PaymentDetailId,
        List<OrderItemRequest> Items);

    public sealed record OrderItemRequest(
        Guid ProductId,
        int Quantity);
}
