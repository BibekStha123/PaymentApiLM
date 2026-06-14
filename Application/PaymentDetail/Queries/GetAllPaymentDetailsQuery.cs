using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetAllPaymentDetailsQuery(int? cursor, int limit) : IRequest<CursorPagedResponse<PaymentDetailResponse>>;
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
                .Where(p => request.cursor == null || p.Id > request.cursor)
                .OrderBy(p => p.Id)
                .Take(request.limit + 1)
                .Select(p => new PaymentDetailResponse(p.Id, p.CardOwnerName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active))
                .ToListAsync(cancellationToken);

            int? nextCursor = null;
            if(items.Count > request.limit)
            {
                items.RemoveAt(items.Count - 1);
                nextCursor = items[^1].Id;
            }

            return new CursorPagedResponse<PaymentDetailResponse>(items, nextCursor);
        }
    }
}
