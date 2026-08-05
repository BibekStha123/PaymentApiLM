using FluentValidation;

namespace PaymentDetailApi.Application.Products.Commands
{
    public class AddStockCommandValidator : AbstractValidator<AddStockCommand>
    {
        public AddStockCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("A valid product id is required.");

            RuleFor(x => x.Stock)
                .GreaterThan(0)
                .WithMessage("Quantity to add must be greater than zero.");
        }
    }
}
