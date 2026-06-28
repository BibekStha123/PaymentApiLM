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
            var items = await _context.PaymentDetails
                .Where(p => p.Active)
                .Where(p => request.cursor == null || p.Id.CompareTo(request.cursor.Value) > 0)
                .OrderBy(p => p.Id)
                .Take(request.limit + 1)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new PaymentDetailResponse(p.Id, u.UserName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active))
                .ToListAsync(cancellationToken);

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
