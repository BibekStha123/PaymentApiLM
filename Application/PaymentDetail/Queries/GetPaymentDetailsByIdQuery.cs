using MediatR;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Infrastructure;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByIdQuery(int id) : IRequest<Domain.Payment.Entities.PaymentDetail>;
    public class GetPaymentDetailsByIdQueryHandler : IRequestHandler<GetPaymentDetailsByIdQuery, Domain.Payment.Entities.PaymentDetail>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByIdQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<Domain.Payment.Entities.PaymentDetail> Handle(GetPaymentDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var paymentDetails = await _context.PaymentDetails.FindAsync(request.id);

            return paymentDetails ?? throw new Exception($"Payment Details does not exist for {request.id}");
        }
    }
}
