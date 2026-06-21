using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByNameQuery(string name) : IRequest<List<PaymentDetailResponse>>;

    public class GetPaymentDetailsByNameQueryHandler : IRequestHandler<GetPaymentDetailsByNameQuery, List<PaymentDetailResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByNameQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDetailResponse>> Handle(GetPaymentDetailsByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.PaymentDetails
                .Where(p => p.Active)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new { p, u })
                .Where(x => x.u.UserName == request.name)
                .Select(x => new PaymentDetailResponse(x.p.Id, x.u.UserName, x.p.CardNumber, x.p.ExpirationDate, x.p.SecurityCode, x.p.Active))
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
