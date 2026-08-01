using PaymentDetailApi.Domain.Orders;

namespace PaymentDetailApi.Application.Orders
{
    public sealed record OrderResponse(
        Guid Id,
        Guid UserId,
        string UserName,
        string ShippingAddress,
        string CurrencyCode,
        OrderStatus Status,
        DateTime OrderDate,
        decimal TotalAmount,
        List<OrderItemResponse> Items);

    public sealed record OrderItemResponse(
        Guid ProductId,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}
