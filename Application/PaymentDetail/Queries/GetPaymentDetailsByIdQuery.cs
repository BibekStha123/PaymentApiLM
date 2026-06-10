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
            var payment = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.Active && p.Id == request.id, cancellationToken);

            if (payment is null)
                throw new Exception($"Payment Details does not exist for {request.id}");

            return new PaymentDetailResponse(payment.Id, payment.CardOwnerName, payment.CardNumber, payment.ExpirationDate, payment.SecurityCode, payment.Active);
        }
    }
}
