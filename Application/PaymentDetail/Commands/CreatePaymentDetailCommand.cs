using MediatR;
using PaymentDetailApi.Infrastructure.DomainEvents;
using PaymentDetailApi.Infrastructure.Persistence;
using DomainPaymentDetail = PaymentDetailApi.Domain.Payment.Entities.PaymentDetail;

namespace PaymentDetailApi.Application.PaymentDetail.Commands
{
    public record CreatePaymentDetailCommand(
        string CardOwnerName,
        string CardNumber,
        string ExpirationDate,
        string SecurityCode) : IRequest<int>;

    public class CreatePaymentDetailCommandHandler : IRequestHandler<CreatePaymentDetailCommand, int>
    {
        private readonly PaymentDetailsContext _context;
        private readonly DomainEventDispatcher _dispatcher;

        public CreatePaymentDetailCommandHandler(PaymentDetailsContext context, DomainEventDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task<int> Handle(CreatePaymentDetailCommand request, CancellationToken cancellationToken)
        {
            var payment = new DomainPaymentDetail(
                request.CardOwnerName,
                request.CardNumber,
                request.ExpirationDate,
                request.SecurityCode);

            _context.PaymentDetails.Add(payment);
            await _context.SaveChangesAsync(cancellationToken); // payment.Id is set after this

            var events = payment.DomainEvents.ToList();
            payment.ClearEvents();
            await _dispatcher.Dispatch(events); // handlers add audit log to context

            await _context.SaveChangesAsync(cancellationToken); // save the audit log

            return payment.Id;
        }
    }
}
