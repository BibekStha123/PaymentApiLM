using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EfUnitOfWorkTransaction(transaction);
        }

        private sealed class EfUnitOfWorkTransaction : IUnitOfWorkTransaction
        {
            private readonly IDbContextTransaction _transaction;

            public EfUnitOfWorkTransaction(IDbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public Task CommitAsync(CancellationToken cancellationToken = default) =>
                _transaction.CommitAsync(cancellationToken);

            public ValueTask DisposeAsync() =>
                _transaction.DisposeAsync();
        }
    }
}
