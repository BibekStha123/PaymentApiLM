using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Repositories;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public record CancelOrderCommand(Guid Id, Guid RequestingUserId, bool IsAdmin) : ICommand<Unit>;

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id {request.Id} not found.");

            if (!request.IsAdmin && order.UserId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You do not have access to this order.");

            order.Cancel();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
