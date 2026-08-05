using FluentValidation;

namespace PaymentDetailApi.Application.Products.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(200)
                .WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Product description is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Product price must be greater than zero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Product stock cannot be negative.");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("A valid category is required.");
        }
    }
}
