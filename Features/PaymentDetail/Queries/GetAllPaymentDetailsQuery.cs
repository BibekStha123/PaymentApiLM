using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Features.PaymentDetail.Queries
{
    public record GetAllPaymentDetailsQuery : IRequest<List<PaymentDetails>>;
    public class GetAllPaymentDetailsQueryHandler : IRequestHandler<GetAllPaymentDetailsQuery, List<PaymentDetails>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllPaymentDetailsQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<List<PaymentDetails>> Handle(GetAllPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentDetails.ToListAsync(cancellationToken);
        }
    }
}
