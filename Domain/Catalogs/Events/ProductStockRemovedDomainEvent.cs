using PaymentDetailApi.Domain.Catalog.Entities;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Events
{
    public class ProductStockRemovedDomainEvent : DomainEvent
    {
        public Product Product { get; }
        public int QuantityRemoved { get; }

        public ProductStockRemovedDomainEvent(Product product, int quantityRemoved)
        {
            Product = product;
            QuantityRemoved = quantityRemoved;
        }
    }
}
