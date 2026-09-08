using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Shared.ValueObjects;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Orders.Queries
{
    public record GetOrderByIdQuery(Guid Id, Guid RequestingUserId, bool IsAdmin) : IRequest<OrderResponse>;

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly PaymentDetailsContext _context;
        public GetOrderByIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Where(o => o.Id == request.Id)
                .Select(o => new
                {
                    o.Id,
                    o.UserId,
                    UserName = _context.Users.Where(u => u.Id == o.UserId).Select(u => u.UserName).FirstOrDefault()!,
                    o.ShippingAddress,
                    CurrencyCode = _context.Currency.Where(c => c.Id == o.CurrencyId).Select(c => c.CurrencyCode).FirstOrDefault()!,
                    o.Status,
                    o.OrderDate,
                    Items = o.OrderItems.Select(i => new { i.ProductId, i.UnitPrice, i.Quantity }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id {request.Id} not found.");

            if (!request.IsAdmin && order.UserId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You do not have access to this order.");

            var orderItems = order.Items
                .Select(i => new OrderItemResponse(i.ProductId, i.UnitPrice.Amount, i.Quantity, i.UnitPrice.Multiply(i.Quantity).Amount))
                .ToList();

            var totalAmount = order.Items.Aggregate(Money.Zero, (sum, i) => sum.Add(i.UnitPrice.Multiply(i.Quantity)));

            return new OrderResponse(
                order.Id,
                order.UserId,
                order.UserName,
                order.ShippingAddress,
                order.CurrencyCode,
                order.Status,
                order.OrderDate,
                totalAmount.Amount,
                orderItems);
        }
    }
}
