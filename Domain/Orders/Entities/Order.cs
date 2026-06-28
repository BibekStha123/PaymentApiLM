using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Events;

namespace PaymentDetailApi.Domain.Orders.Entities
{
    public class Order : AggregateRoot
    {
        private readonly List<OrderItem> _orderItems = new();

        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public string ShippingAddress { get; private set; }
        public Guid CurrencyId { get; private set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public decimal TotalAmount => _orderItems.Sum(i => i.TotalPrice);

        private Order() { }

        private Order(Guid userId, string shippingAddress, Guid currencyId)
        {
            Validate(userId, shippingAddress, currencyId);
            UserId = userId;
            ShippingAddress = shippingAddress;
            CurrencyId = currencyId;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;

            AddDomainEvent(new OrderCreatedDomainEvent(userId, this));
        }

        public static Order Create(Guid userId, string shippingAddress, Guid currencyId)
            => new(userId, shippingAddress, currencyId);

        public void AddItem(Guid productId, decimal unitPrice, int quantity)
        {
            var item = OrderItem.Create(Id, productId, unitPrice, quantity);
            _orderItems.Add(item);
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order.");

            Status = OrderStatus.Cancelled;
        }

        private static void Validate(Guid userId, string shippingAddress, Guid currencyId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("A valid user is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new ArgumentException("Shipping address is required.", nameof(shippingAddress));

            if (currencyId == Guid.Empty)
                throw new ArgumentException("A valid currency is required.", nameof(currencyId));
        }
    }
}
