using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.Common.Interfaces;
using PaymentDetailApi.Application.User;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.Application.Users.Commands
{
    public record LoginUserCommand(string Email, string Password) : ICommand<UserResponse>;

    public class LoginUserHandler : IRequestHandler<LoginUserCommand, UserResponse>
    {
        private readonly PaymentDetailsContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserHandler(PaymentDetailsContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<UserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(user.Id, user.UserName, user.Email, user.Role);

            return new UserResponse(user.Email, user.DisplayName, token);
        }
    }
}
