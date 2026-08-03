using FluentValidation;

namespace PaymentDetailApi.Application.Orders.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.CurrencyId)
                .NotEmpty();

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("An order must contain at least one item.");

            RuleForEach(x => x.Items).SetValidator(new CreateOrderItemCommandValidator());
        }
    }

    public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
    {
        public CreateOrderItemCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}
