using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
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
            var items = await _context.Orders
                .Where(o => request.Cursor == null || o.Id.CompareTo(request.Cursor.Value) > 0)
                .OrderBy(o => o.Id)
                .Take(request.Limit + 1)
                .Select(o => new OrderResponse(
                    o.Id,
                    o.UserId,
                    _context.Users.Where(u => u.Id == o.UserId).Select(u => u.UserName).FirstOrDefault()!,
                    o.ShippingAddress,
                    _context.Currency.Where(c => c.Id == o.CurrencyId).Select(c => c.CurrencyCode).FirstOrDefault()!,
                    o.Status,
                    o.OrderDate,
                    o.OrderItems.Sum(i => i.UnitPrice * i.Quantity),
                    o.OrderItems.Select(i => new OrderItemResponse(i.ProductId, i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity)).ToList()
                ))
                .ToListAsync(cancellationToken);

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
