using PaymentDetailApi.Domain.Catalog.Entities;

namespace PaymentDetailApi.Domain.Catalog.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
    }
}
