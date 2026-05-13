using MediatR;
using PaymentDetailApi.Infrastructure;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Features.PaymentDetail.Queries
{
    public record GetPaymentDetailsByIdQuery(int id) : IRequest<PaymentDetails>;
    public class GetPaymentDetailsByIdQueryHandler : IRequestHandler<GetPaymentDetailsByIdQuery, PaymentDetails>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<PaymentDetails> Handle(GetPaymentDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var paymentDetails = await _context.PaymentDetails.FindAsync(request.id);

            return paymentDetails ?? throw new Exception($"Payment Details does not exist for {request.id}");
        }
    }
}
