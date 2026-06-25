using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Products
{
    public class ProductStockAddedEventHandler : IDomainEventHandler<ProductStockAddedDomainEvent>
    {
        private readonly PaymentDetailsContext _context;
        public ProductStockAddedEventHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task Handle(ProductStockAddedDomainEvent domainEvent)
        {
            _context.Logs.Add(Log.Create(
                "Stock Added",
                "Product",
                domainEvent.Product.Id,
                $"{domainEvent.QuantityAdded} Stock added for Product {domainEvent.Product.Name}"
            ));

            await Task.CompletedTask;
        }
    }
}
