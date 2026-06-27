namespace PaymentDetailApi.Domain.Orders.Entities
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public Guid OrderId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem() { }

        private OrderItem(Guid orderId, int productId, string productName, decimal unitPrice, int quantity)
        {
            Validate(productId, productName, unitPrice, quantity);
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        internal static OrderItem Create(Guid orderId, int productId, string productName, decimal unitPrice, int quantity)
            => new(orderId, productId, productName, unitPrice, quantity);

        private static void Validate(int productId, string productName, decimal unitPrice, int quantity)
        {
            if (productId <= 0)
                throw new ArgumentException("A valid product is required.", nameof(productId));

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required.", nameof(productName));

            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
    }
}
