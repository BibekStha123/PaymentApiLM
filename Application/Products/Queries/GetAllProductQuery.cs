using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Products.Queries
{
    public record GetAllProductQuery(Guid? Cursor, int Limit) : IRequest<CursorPagedResponse<ProductResponse>>;

    public class GetAllProductQueryHanlder : IRequestHandler<GetAllProductQuery, CursorPagedResponse<ProductResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllProductQueryHanlder(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<CursorPagedResponse<ProductResponse>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            // Select the whole converted Price property first (EF can translate that), then read
            // .Amount off it in memory - .Amount access can't be translated directly in the query.
            var rows = await _context.Products
                .Where(p => request.Cursor == null || p.Id.CompareTo(request.Cursor.Value) > 0)
                .OrderBy(p => p.Id)
                .Take(request.Limit + 1)
                .Join(_context.Categories,
                      p => p.CategoryId,
                      c => c.Id,
                      (p, c) => new { p.Id, p.Name, p.Description, p.Price, p.Stock, CategoryName = c.Name })
                .ToListAsync(cancellationToken);

            var items = rows
                .Select(r => new ProductResponse(r.Id, r.Name, r.Description, r.Price.Amount, r.Stock, r.CategoryName))
                .ToList();

            Guid? nextCursor = null;
            if (items.Count > request.Limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<ProductResponse>(items, nextCursor);
        }
    }

}
