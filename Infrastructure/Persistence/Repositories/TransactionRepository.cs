using PaymentDetailApi.Domain.Transactions.Entities;
using PaymentDetailApi.Domain.Transactions.Repositories;

namespace PaymentDetailApi.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PaymentDetailsContext _context;

        public TransactionRepository(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default) =>
            await _context.Transactions.AddAsync(transaction, cancellationToken);
    }
}
