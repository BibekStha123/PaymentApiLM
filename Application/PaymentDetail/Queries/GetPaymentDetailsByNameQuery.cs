using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Infrastructure;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetPaymentDetailsByNameQuery(string name) : IRequest<Domain.Payment.Entities.PaymentDetail>;
    public class GetPaymentDetailsByNameQueryHandler : IRequestHandler<GetPaymentDetailsByNameQuery, Domain.Payment.Entities.PaymentDetail>
    {
        private readonly PaymentDetailsContext _context;
        public GetPaymentDetailsByNameQueryHandler(PaymentDetailsContext context    )
        {
            _context = context;
        }

        public async Task<Domain.Payment.Entities.PaymentDetail> Handle(GetPaymentDetailsByNameQuery request, CancellationToken cancellationToken)
        {
            var paymentDetails = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.CardOwnerName == request.name);

            return paymentDetails ?? throw new Exception($"Payment Details does not exist for {request.name}");

        }
    }
}
