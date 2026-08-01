using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Products
{
    public class ProductStockRemovedEventHandler : IDomainEventHandler<ProductStockRemovedDomainEvent>
    {
        private readonly PaymentDetailsContext _context;
        public ProductStockRemovedEventHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task Handle(ProductStockRemovedDomainEvent domainEvent)
        {
            _context.Logs.Add(Log.Create(
                "Stock Removed",
                "Product",
                domainEvent.Product.Id,
                $"{domainEvent.QuantityRemoved} Stock removed for Product {domainEvent.Product.Name}"
            ));

            await Task.CompletedTask;
        }
    }
}
