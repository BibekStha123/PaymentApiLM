namespace PaymentDetailApi.API.Controllers.Orders
{
    public sealed record OrderRequest(
        string ShippingAddress,
        Guid CurrencyId,
        List<OrderItemRequest> Items);

    public sealed record OrderItemRequest(
        Guid ProductId,
        int Quantity);
}
