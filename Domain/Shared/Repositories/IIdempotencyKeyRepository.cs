namespace PaymentDetailApi.Domain.Shared.Repositories
{
    public interface IIdempotencyKeyRepository
    {
        Task AddAsync(IdempotencyKey key, CancellationToken cancellationToken = default);
        Task<IdempotencyKey?> FindByKeyAsync(Guid userId, string key, CancellationToken cancellationToken = default);
        void Detach(IdempotencyKey key);
    }
}
