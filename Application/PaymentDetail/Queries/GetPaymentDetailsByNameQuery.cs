using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByNameQuery(string name) : IRequest<PaymentDetailResponse>;
    public class GetPaymentDetailsByNameQueryHandler : IRequestHandler<GetPaymentDetailsByNameQuery, PaymentDetailResponse>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByNameQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<PaymentDetailResponse> Handle(GetPaymentDetailsByNameQuery request, CancellationToken cancellationToken)
        {
            var payment = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.Active && p.CardOwnerName == request.name, cancellationToken);

            if (payment is null)
                throw new Exception($"Payment Details does not exist for {request.name}");

            return new PaymentDetailResponse(payment.Id, payment.CardOwnerName, payment.CardNumber, payment.ExpirationDate, payment.SecurityCode, payment.Active);
        }
    }
}
