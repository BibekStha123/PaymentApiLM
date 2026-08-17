using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByIdQuery(Guid id) : IRequest<PaymentDetailResponse>;
    public class GetPaymentDetailsByIdQueryHandler : IRequestHandler<GetPaymentDetailsByIdQuery, PaymentDetailResponse>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<PaymentDetailResponse> Handle(GetPaymentDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var row = await _context.PaymentDetails
                .Where(p => p.Active && p.Id == request.id)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.Id,
                    (p, u) => new { p.Id, u.UserName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active })
                .FirstOrDefaultAsync(cancellationToken);

            if (row is null)
                throw new Exception($"Payment Details does not exist for {request.id}");

            return new PaymentDetailResponse(row.Id, row.UserName, row.CardNumber.Value, row.ExpirationDate.Value, row.SecurityCode, row.Active);
        }
    }
}
