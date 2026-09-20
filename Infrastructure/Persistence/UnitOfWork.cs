using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDetailsContext _context;

        public UnitOfWork(PaymentDetailsContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);
    }
}
