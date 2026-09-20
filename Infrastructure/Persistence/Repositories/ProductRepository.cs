using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Catalog.Repositories;

namespace PaymentDetailApi.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PaymentDetailsContext _context;

        public ProductRepository(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _context.Products.FindAsync(id, cancellationToken);

        public Task<Product?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Products
                .FromSqlInterpolated($"SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}")
                .FirstOrDefaultAsync(cancellationToken);

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
            await _context.Products.AddAsync(product, cancellationToken);
    }
}
