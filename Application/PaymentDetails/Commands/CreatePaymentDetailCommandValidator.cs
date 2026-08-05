using FluentValidation;
using PaymentDetailApi.Application.PaymentDetail.Commands;

namespace PaymentDetailApi.Application.PaymentDetails.Commands
{
    public class CreatePaymentDetailCommandValidator : AbstractValidator<CreatePaymentDetailCommand>
    {
        public CreatePaymentDetailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .WithMessage("Card number is required.")
                .Matches(@"^\d{16}$")
                .WithMessage("Card number must be exactly 16 digits.");

            RuleFor(x => x.ExpirationDate)
                .NotEmpty()
                .WithMessage("Expiration date is required.")
                .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$")
                .WithMessage("Expiration date must be in MM/YY format.");

            RuleFor(x => x.SecurityCode)
                .NotEmpty()
                .WithMessage("Security code is required.")
                .Matches(@"^\d{3,4}$")
                .WithMessage("Security code must be 3 or 4 digits.");
        }
    }
}
