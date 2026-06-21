using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByUserIdQuery(Guid UserId) : IRequest<List<PaymentDetailResponse>>;

    public class GetPaymentDetailsByUserIdQueryHandler : IRequestHandler<GetPaymentDetailsByUserIdQuery, List<PaymentDetailResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByUserIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDetailResponse>> Handle(GetPaymentDetailsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.PaymentDetails
                .Where(p => p.Active && p.UserId == request.UserId)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new PaymentDetailResponse(p.Id, u.UserName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active))
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
