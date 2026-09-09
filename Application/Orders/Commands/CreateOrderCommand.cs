using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public record CreateOrderCommand(
        Guid UserId,
        string ShippingAddress,
        Guid CurrencyId,
        List<CreateOrderItemCommand> Items,
        string? IdempotencyKey = null) : ICommand<Guid>;

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
            IdempotencyKey? claim = null;

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                claim = IdempotencyKey.Create(request.UserId, request.IdempotencyKey);
                await _dbContext.IdempotencyKeys.AddAsync(claim, cancellationToken);

                try
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    _dbContext.Entry(claim).State = EntityState.Detached;

                    var existing = await _dbContext.IdempotencyKeys
                        .FirstOrDefaultAsync(k => k.UserId == request.UserId && k.Key == request.IdempotencyKey, cancellationToken);

                    if (existing?.OrderId is Guid existingOrderId)
                        return existingOrderId;

                    throw new InvalidOperationException("A request with this idempotency key is already being processed.");
                }
            }

            var order = Order.Create(request.UserId, request.ShippingAddress, request.CurrencyId);

            foreach (var item in request.Items)
            {
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken)
                    ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

                product.RemoveStock(item.Quantity);
                order.AddItem(product.Id, product.Price, item.Quantity);
            }

            await _dbContext.Orders.AddAsync(order, cancellationToken);

            claim?.AttachOrder(order.Id);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
