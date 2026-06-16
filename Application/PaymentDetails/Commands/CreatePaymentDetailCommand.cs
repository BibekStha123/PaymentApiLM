using MediatR;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;
using DomainPaymentDetail = PaymentDetailApi.Domain.Payment.Entities.PaymentDetail;

namespace PaymentDetailApi.Application.PaymentDetail.Commands
{
    public record CreatePaymentDetailCommand(
        string CardOwnerName,
        string CardNumber,
        string ExpirationDate,
        string SecurityCode) : ICommand<int>;

    public class CreatePaymentDetailCommandHandler : IRequestHandler<CreatePaymentDetailCommand, int>
    {
        private readonly PaymentDetailsContext _context;

        public CreatePaymentDetailCommandHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreatePaymentDetailCommand request, CancellationToken cancellationToken)
        {
            var payment = new DomainPaymentDetail(
                request.CardOwnerName,
                request.CardNumber,
                request.ExpirationDate,
                request.SecurityCode);

            _context.PaymentDetails.Add(payment);
            await _context.SaveChangesAsync(cancellationToken); // payment.Id set; events still on entity

            return payment.Id;
        }
    }
}
