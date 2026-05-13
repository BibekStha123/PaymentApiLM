using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Features.PaymentDetail.Queries
{
    public record GetPaymentDetailsByNameQuery(string name) : IRequest<PaymentDetails>;
    public class GetPaymentDetailsByNameQueryHandler : IRequestHandler<GetPaymentDetailsByNameQuery, PaymentDetails>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByNameQueryHandler(PaymentDetailsContext context    )
        {
            _context = context;
        }

        public async Task<PaymentDetails> Handle(GetPaymentDetailsByNameQuery request, CancellationToken cancellationToken)
        {
            var paymentDetails = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.CardOwnerName == request.name);

            return paymentDetails ?? throw new Exception($"Payment Details does not exist for {request.name}");

        }
    }
}
