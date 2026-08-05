using FluentValidation;

namespace PaymentDetailApi.Application.Categories.Commands
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(100)
                .WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Category type is required.")
                .MaximumLength(50)
                .WithMessage("Category type must not exceed 50 characters.");
        }
    }
}
