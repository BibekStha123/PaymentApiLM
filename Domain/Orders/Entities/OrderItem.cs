using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Orders.Entities
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem() { }

        private OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity)
        {
            Validate(productId, unitPrice, quantity);
            OrderId = orderId;
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        internal static OrderItem Create(Guid orderId, Guid productId, decimal unitPrice, int quantity)
            => new(orderId, productId, unitPrice, quantity);

        private static void Validate(Guid productId, decimal unitPrice, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("A valid product is required.", nameof(productId));

            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
    }
}
