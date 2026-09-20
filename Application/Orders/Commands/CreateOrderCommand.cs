using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Catalog.Repositories;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Entities;
using PaymentDetailApi.Domain.Orders.Repositories;
using PaymentDetailApi.Domain.Payment.Repositories;
using PaymentDetailApi.Domain.Shared;
using PaymentDetailApi.Domain.Shared.Repositories;
using PaymentDetailApi.Domain.Transactions.Entities;
using PaymentDetailApi.Domain.Transactions.Repositories;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public record CreateOrderCommand(
        Guid UserId,
        string ShippingAddress,
        Guid CurrencyId,
        Guid PaymentDetailId,
        List<CreateOrderItemCommand> Items,
        string? IdempotencyKey = null) : ICommand<Guid>;

    public record CreateOrderItemCommand(
        Guid ProductId,
        int Quantity);

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IIdempotencyKeyRepository _idempotencyKeyRepository;
        private readonly IPaymentDetailRepository _paymentDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IIdempotencyKeyRepository idempotencyKeyRepository,
            IPaymentDetailRepository paymentDetailRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _idempotencyKeyRepository = idempotencyKeyRepository;
            _paymentDetailRepository = paymentDetailRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            IdempotencyKey? claim = null;

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                claim = IdempotencyKey.Create(request.UserId, request.IdempotencyKey);
                await _idempotencyKeyRepository.AddAsync(claim, cancellationToken);

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    _idempotencyKeyRepository.Detach(claim);

                    var existing = await _idempotencyKeyRepository.FindByKeyAsync(request.UserId, request.IdempotencyKey, cancellationToken);

                    if (existing?.OrderId is Guid existingOrderId)
                        return existingOrderId;

                    throw new InvalidOperationException("A request with this idempotency key is already being processed.");
                }
            }

            var paymentDetail = await _paymentDetailRepository.FindActiveForUserAsync(request.PaymentDetailId, request.UserId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment detail {request.PaymentDetailId} not found.");

            var order = Order.Create(request.UserId, request.ShippingAddress, request.CurrencyId);

            await using var dbTransaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            foreach (var item in request.Items)
            {
                //pessimistic locking, lock the product from race condition
                var product = await _productRepository.GetForUpdateAsync(item.ProductId, cancellationToken)
                    ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

                product.RemoveStock(item.Quantity);
                order.AddItem(product.Id, product.Price, item.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            var transaction = Transaction.Create(order.Id, paymentDetail.Id, order.TotalAmount, request.CurrencyId);
            await _transactionRepository.AddAsync(transaction, cancellationToken);

            claim?.AttachOrder(order.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            return order.Id;
        }
    }
}
