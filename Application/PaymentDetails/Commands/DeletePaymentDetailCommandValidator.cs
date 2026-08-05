using FluentValidation;
using PaymentDetailApi.Application.PaymentDetail.Commands;

namespace PaymentDetailApi.Application.PaymentDetails.Commands
{
    public class DeletePaymentDetailCommandValidator : AbstractValidator<DeletePaymentDetailCommand>
    {
        public DeletePaymentDetailCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0)
                .WithMessage("A valid payment detail id is required.");
        }
    }
}
