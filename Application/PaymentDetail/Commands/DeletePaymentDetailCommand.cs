using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.PaymentDetail.Commands
{
    public record DeletePaymentDetailCommand(int id) : ICommand<string>;

    public class DeletePaymentDetailCommandHandler : IRequestHandler<DeletePaymentDetailCommand, string>
    {
        private readonly PaymentDetailsContext _context;
        public DeletePaymentDetailCommandHandler(PaymentDetailsContext context)
        {
            _context = context;
        }
        public async Task<string> Handle(DeletePaymentDetailCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.PaymentDetails.FindAsync(request.id, cancellationToken);
            if (payment is null)
                return $"Payment with id {request.id} not found.";

            payment.Delete();
            //_context.PaymentDetails.Remove(payment);

            return $"Payment with id {request.id} deleted successfully.";
        }
    }
}
