using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Infrastructure.Persistence;
using DomainUser = PaymentDetailApi.Domain.User.Entities.User;

namespace PaymentDetailApi.Application.Users.Commands
{
    public record RegisterUserCommand(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? DisplayName) : ICommand<Guid>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly PaymentDetailsContext _context;

        public RegisterUserCommandHandler(PaymentDetailsContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await ValidateUniqueness(request, cancellationToken);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            var user = DomainUser.Register(
                request.UserName,
                request.Email,
                passwordHash,
                request.DisplayName);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
        private async Task ValidateUniqueness(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);
            if (emailTaken)
                throw new InvalidOperationException("Email is already registered.");

            var userNameTaken = await _context.Users
                .AnyAsync(u => u.UserName == request.UserName, cancellationToken);
            if (userNameTaken)
                throw new InvalidOperationException("Username is already taken.");
        }
    }
}
