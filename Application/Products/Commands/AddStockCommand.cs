using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Catalog.Repositories;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Application.Products.Commands
{
    public record AddStockCommand(Guid Id, int Stock) : ICommand<int>;

    public class AddStockCommandHanlder : IRequestHandler<AddStockCommand, int>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddStockCommandHanlder(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
                throw new KeyNotFoundException($"Product with id {request.Id} not found.");

            product.AddStock(request.Stock);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Stock;
        }
    }
}
