using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByIdQuery(int id) : IRequest<PaymentDetailResponse>;
    public class GetPaymentDetailsByIdQueryHandler : IRequestHandler<GetPaymentDetailsByIdQuery, PaymentDetailResponse>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<PaymentDetailResponse> Handle(GetPaymentDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.PaymentDetails
                .Where(p => p.Active && p.Id == request.id)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new PaymentDetailResponse(p.Id, u.UserName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active))
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                throw new Exception($"Payment Details does not exist for {request.id}");

            return result;
        }
    }
}
