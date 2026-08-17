using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetAllPaymentDetailsQuery(Guid? cursor, int limit) : IRequest<CursorPagedResponse<PaymentDetailResponse>>;
    public class GetAllPaymentDetailsQueryHandler : IRequestHandler<GetAllPaymentDetailsQuery, CursorPagedResponse<PaymentDetailResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllPaymentDetailsQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<CursorPagedResponse<PaymentDetailResponse>> Handle(GetAllPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            // Materialize the whole CardNumber/ExpirationDate value objects first - EF Core can
            // translate selecting a converted property as a whole, but not member access (.Value)
            // on it within the query itself, so the .Value projection has to happen in memory after.
            var rows = await _context.PaymentDetails
                .Where(p => p.Active)
                .Where(p => request.cursor == null || p.Id.CompareTo(request.cursor.Value) > 0)
                .OrderBy(p => p.Id)
                .Take(request.limit + 1)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new { p.Id, u.UserName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active })
                .ToListAsync(cancellationToken);

            var items = rows
                .Select(r => new PaymentDetailResponse(r.Id, r.UserName, r.CardNumber.Value, r.ExpirationDate.Value, r.SecurityCode, r.Active))
                .ToList();

            Guid? nextCursor = null;
            if(items.Count > request.limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<PaymentDetailResponse>(items, nextCursor);
        }
    }
}
