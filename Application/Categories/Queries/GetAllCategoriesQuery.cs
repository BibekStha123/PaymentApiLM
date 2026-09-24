using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Categories.Queries
{
    public record GetAllCategoriesQuery(Guid? Cursor, int Limit) : IRequest<CursorPagedResponse<CategoryResponse>>;

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, CursorPagedResponse<CategoryResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllCategoriesQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<CursorPagedResponse<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Categories
                .Where(c => request.Cursor == null || c.Id.CompareTo(request.Cursor.Value) > 0)
                .OrderBy(c => c.Id)
                .Take(request.Limit + 1)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Type))
                .ToListAsync(cancellationToken);

            Guid? nextCursor = null;
            if (items.Count > request.Limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<CategoryResponse>(items, nextCursor);
        }
    }
}
