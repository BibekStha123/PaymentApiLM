using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Currency.Queries
{
    public record GetAllCurrencyQuery(int? Cursor, int Limit) : IRequest<CursorPagedResponse<CurrencyResponse>>;

    public class GetAllCurrencyQueryHandler : IRequestHandler<GetAllCurrencyQuery, CursorPagedResponse<CurrencyResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllCurrencyQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<CursorPagedResponse<CurrencyResponse>> Handle(GetAllCurrencyQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Currency
                .Where(c => request.Cursor == null || c.Id > request.Cursor)
                .OrderBy(c => c.Id)
                .Take(request.Limit + 1)
                .Select(c => new CurrencyResponse(c.Id, c.CurrencyCode, c.Name))
                .ToListAsync(cancellationToken);

            int? nextCursor = null;
            if (items.Count > request.Limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<CurrencyResponse>(items, nextCursor);
        }
    }
}