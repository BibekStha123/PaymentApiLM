using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Products.Commands
{
    public record CreateProductCommand
    (
        string Name,
        string Description,
        decimal Price,
        int Stock,
        Guid CategoryId,
        bool IsActive
    ) : ICommand<Guid>;

    public class CreateProductCommandHanlder : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly PaymentDetailsContext _dbContext;
        public CreateProductCommandHanlder(PaymentDetailsContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = Product.Create(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.CategoryId,
                request.IsActive
            );

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }

}
