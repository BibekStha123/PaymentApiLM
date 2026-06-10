using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetAllPaymentDetailsQuery : IRequest<List<PaymentDetailResponse>>;
    public class GetAllPaymentDetailsQueryHandler : IRequestHandler<GetAllPaymentDetailsQuery, List<PaymentDetailResponse>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllPaymentDetailsQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<List<PaymentDetailResponse>> Handle(GetAllPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentDetails
                .Where(p => p.Active)
                .Select(p => new PaymentDetailResponse(p.Id, p.CardOwnerName, p.CardNumber, p.ExpirationDate, p.SecurityCode, p.Active))
                .ToListAsync(cancellationToken);
        }
    }
}
