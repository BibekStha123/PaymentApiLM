using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Entities;

namespace PaymentDetailApi.Domain.Orders.Events
{
    public class OrderCreatedDomainEvent : DomainEvent
    {
        public Order Order { get; }
        public Guid UserId { get; }
        public OrderCreatedDomainEvent(Guid userId, Order order)
        {
            UserId = userId;
            Order = order;
        }
    }
}
