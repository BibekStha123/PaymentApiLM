using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Orders.Entities
{
    public class Order : AggregateRoot
    {
        private readonly List<OrderItem> _orderItems = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public string ShippingAddress { get; private set; }
        public int CurrencyId { get; private set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public decimal TotalAmount => _orderItems.Sum(i => i.TotalPrice);

        private Order() { }

        private Order(Guid userId, string shippingAddress, int currencyId)
        {
            Validate(userId, shippingAddress, currencyId);
            Id = Guid.CreateVersion7();
            UserId = userId;
            ShippingAddress = shippingAddress;
            CurrencyId = currencyId;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;
        }

        public static Order Create(Guid userId, string shippingAddress, int currencyId)
            => new(userId, shippingAddress, currencyId);

        public void AddItem(int productId, string productName, decimal unitPrice, int quantity)
        {
            var item = OrderItem.Create(Id, productId, productName, unitPrice, quantity);
            _orderItems.Add(item);
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order.");

            Status = OrderStatus.Cancelled;
        }

        private static void Validate(Guid userId, string shippingAddress, int currencyId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("A valid user is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new ArgumentException("Shipping address is required.", nameof(shippingAddress));

            if (currencyId <= 0)
                throw new ArgumentException("A valid currency is required.", nameof(currencyId));
        }
    }
}
