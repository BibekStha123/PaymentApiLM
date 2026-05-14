using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.PaymentDetail.Events;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Features.PaymentDetail.Events;
using PaymentDetailApi.Infrastructure;

namespace PaymentDetailApi.Application.PaymentDetail.Commands
{
    public record CreatePaymentDetailCommand(Domain.Payment.Entities.PaymentDetail paymentDetail) : IRequest<int>;
    public class CreatePaymentDetailCommandHandler : IRequestHandler<CreatePaymentDetailCommand, int>
    {
        private readonly PaymentDetailsContext _context;
        private readonly IMediator _mediator;
        public CreatePaymentDetailCommandHandler(PaymentDetailsContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<int> Handle(CreatePaymentDetailCommand request, CancellationToken cancellationToken)
        {
            _context.PaymentDetails.Add(request.paymentDetail);

            await _mediator.Publish(new PaymentCreatedEvent(request.paymentDetail), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return request.paymentDetail.Id;
        }
    }
}
