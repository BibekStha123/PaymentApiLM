using PaymentDetailApi.Domain.Transactions.Entities;

namespace PaymentDetailApi.Domain.Transactions.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    }
}
