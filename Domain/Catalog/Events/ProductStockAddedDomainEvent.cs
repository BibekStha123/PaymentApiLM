using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Events
{
    public class ProductStockAddedDomainEvent : DomainEvent
    {
        public Product Product { get; }
        public int QuantityAdded { get; }

        public ProductStockAddedDomainEvent(Product product, int quantityAdded)
        {
            Product = product;
            QuantityAdded = quantityAdded;
        }
    }
}
