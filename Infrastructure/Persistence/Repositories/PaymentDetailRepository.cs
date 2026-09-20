using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Domain.Payment.Repositories;

namespace PaymentDetailApi.Infrastructure.Persistence.Repositories
{
    public class PaymentDetailRepository : IPaymentDetailRepository
    {
        private readonly PaymentDetailsContext _context;

        public PaymentDetailRepository(PaymentDetailsContext context)
        {
            _context = context;
        }

        public Task<PaymentDetail?> FindActiveForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default) =>
            _context.PaymentDetails.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && p.Active, cancellationToken);
    }
}
