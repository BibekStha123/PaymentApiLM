using FluentValidation;

namespace PaymentDetailApi.Application.Users.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(50)
                .WithMessage("Username must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Password and confirmation password do not match.");

            RuleFor(x => x.DisplayName)
                .MaximumLength(100)
                .WithMessage("Display name must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));
        }
    }
}
