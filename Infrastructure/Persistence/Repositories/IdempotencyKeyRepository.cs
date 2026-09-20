using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Domain.Shared.Repositories;

namespace PaymentDetailApi.Infrastructure.Persistence.Repositories
{
    public class IdempotencyKeyRepository : IIdempotencyKeyRepository
    {
        private readonly PaymentDetailsContext _context;

        public IdempotencyKeyRepository(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IdempotencyKey key, CancellationToken cancellationToken = default) =>
            await _context.IdempotencyKeys.AddAsync(key, cancellationToken);

        public Task<IdempotencyKey?> FindByKeyAsync(Guid userId, string key, CancellationToken cancellationToken = default) =>
            _context.IdempotencyKeys.FirstOrDefaultAsync(k => k.UserId == userId && k.Key == key, cancellationToken);

        public void Detach(IdempotencyKey key) =>
            _context.Entry(key).State = EntityState.Detached;
    }
}
