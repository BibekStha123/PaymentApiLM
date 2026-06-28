using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public record CreateOrderCommand(
        Guid UserId,
        string ShippingAddress,
        Guid CurrencyId,
        List<CreateOrderItemCommand> Items) : ICommand<Guid>;

    public record CreateOrderItemCommand(
        Guid ProductId,
        int Quantity);

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly PaymentDetailsContext _dbContext;

        public CreateOrderCommandHandler(PaymentDetailsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(request.UserId, request.ShippingAddress, request.CurrencyId);

            foreach (var item in request.Items)
            {
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken)
                    ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

                order.AddItem(product.Id, product.Price, item.Quantity);
            }

            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
