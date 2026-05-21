using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Queries
{
    public record GetAllPaymentDetailsQuery : IRequest<List<Domain.Payment.Entities.PaymentDetail>>;
    public class GetAllPaymentDetailsQueryHandler : IRequestHandler<GetAllPaymentDetailsQuery, List<Domain.Payment.Entities.PaymentDetail>>
    {
        private readonly PaymentDetailsContext _context;
        public GetAllPaymentDetailsQueryHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<List<Domain.Payment.Entities.PaymentDetail>> Handle(GetAllPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentDetails.FromSqlRaw("SelectAllPaymentDetails").ToListAsync();
        }
    }
}
