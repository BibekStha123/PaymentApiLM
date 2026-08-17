using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared.ValueObjects;

namespace PaymentDetailApi.Domain.Orders.Entities
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public Money UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }
        public Money TotalPrice => UnitPrice.Multiply(Quantity);

        private OrderItem() { }

        private OrderItem(Guid orderId, Guid productId, Money unitPrice, int quantity)
        {
            Validate(productId, quantity);
            OrderId = orderId;
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        internal static OrderItem Create(Guid orderId, Guid productId, Money unitPrice, int quantity)
            => new(orderId, productId, unitPrice, quantity);

        private static void Validate(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("A valid product is required.", nameof(productId));

            // UnitPrice > 0 is already guaranteed by Money.Create().

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
    }
}
