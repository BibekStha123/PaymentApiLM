using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public record CancelOrderCommand(Guid Id, Guid RequestingUserId, bool IsAdmin) : ICommand<Unit>;

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly PaymentDetailsContext _context;
        public CancelOrderCommandHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id {request.Id} not found.");

            if (!request.IsAdmin && order.UserId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You do not have access to this order.");

            order.Cancel();
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
