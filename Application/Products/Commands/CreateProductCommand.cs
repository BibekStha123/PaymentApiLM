using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Catalog.Repositories;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared.ValueObjects;

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
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProductCommandHanlder(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = Product.Create(
                request.Name,
                request.Description,
                Money.Create(request.Price),
                request.Stock,
                request.CategoryId,
                request.IsActive
            );

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }

}
