using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Shared.ValueObjects;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Orders.Queries
{
    public record GetAllOrderQuery(Guid? Cursor, int Limit) : IRequest<CursorPagedResponse<OrderResponse>>;

    public class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, CursorPagedResponse<OrderResponse>>
    {
        private readonly PaymentDetailsContext _context;

        public GetAllOrderQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<CursorPagedResponse<OrderResponse>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var rows = await _context.Orders
                .Where(o => request.Cursor == null || o.Id.CompareTo(request.Cursor.Value) > 0)
                .OrderBy(o => o.Id)
                .Take(request.Limit + 1)
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
                .ToListAsync(cancellationToken);

            var items = rows.Select(o =>
            {
                var orderItems = o.Items
                    .Select(i => new OrderItemResponse(i.ProductId, i.UnitPrice.Amount, i.Quantity, i.UnitPrice.Multiply(i.Quantity).Amount))
                    .ToList();

                var totalAmount = o.Items.Aggregate(Money.Zero, (sum, i) => sum.Add(i.UnitPrice.Multiply(i.Quantity)));

                return new OrderResponse(
                    o.Id,
                    o.UserId,
                    o.UserName,
                    o.ShippingAddress,
                    o.CurrencyCode,
                    o.Status,
                    o.OrderDate,
                    totalAmount.Amount,
                    orderItems);
            }).ToList();

            Guid? nextCursor = null;
            if (items.Count > request.Limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<OrderResponse>(items, nextCursor);
        }
    }
}
