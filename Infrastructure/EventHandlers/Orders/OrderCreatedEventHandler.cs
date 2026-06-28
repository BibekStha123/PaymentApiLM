using PaymentDetailApi.Application.Common.Interfaces;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Events;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Infrastructure.EventHandlers.Orders
{
    public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedDomainEvent>
    {
        private readonly IEmailService _emailService;
        private readonly PaymentDetailsContext _context;
        public OrderCreatedEventHandler(IEmailService emailService, PaymentDetailsContext context)
        {
            _emailService = emailService;
            _context = context;
        }
        public async Task Handle(OrderCreatedDomainEvent domainEvent)
        {
            await _emailService.SendAsync();

            _context.Logs.Add(Log.Create(
                "Order Created",
                "Order",
                domainEvent.Order.Id,
                $"Order {domainEvent.Order.Id} Created for User {domainEvent.UserId}"
            ));
        }
    }
}
