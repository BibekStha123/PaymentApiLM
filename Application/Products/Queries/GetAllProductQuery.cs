using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Products.Queries
{
    public record GetAllProductQuery(int? Cursor, int Limit) : IRequest<CursorPagedResponse<ProductResponse>>;

    public class GetAllProductQueryHanlder : IRequestHandler<GetAllProductQuery, CursorPagedResponse<ProductResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllProductQueryHanlder(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<CursorPagedResponse<ProductResponse>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Products
                .Where(p => request.Cursor == null || p.Id > request.Cursor)
                .OrderBy(p => p.Id)
                .Take(request.Limit + 1)
                .Join(_context.Categories,
                      p => p.CategoryId,
                      c => c.Id,
                      (p, c) => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock, c.Name))
                .ToListAsync(cancellationToken);

            int? nextCursor = null;
            if (items.Count > request.Limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<ProductResponse>(items, nextCursor);
        }
    }

}
