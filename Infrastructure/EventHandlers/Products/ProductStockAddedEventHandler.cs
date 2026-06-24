using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Products
{
    public class ProductStockAddedEventHandler : IDomainEventHandler<ProductStockAddedDomainEvent>
    {
        public async Task Handle(ProductStockAddedDomainEvent domainEvent)
        {
            Console.WriteLine($"{domainEvent.QuantityAdded} {domainEvent.Product.Name} Product stock added.");

            await Task.CompletedTask;
        }
    }
}
