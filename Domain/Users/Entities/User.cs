using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.User.ValueObjects;
using PaymentDetailApi.Domain.Users.Events;

namespace PaymentDetailApi.Domain.User.Entities
{
    public class User : AggregateRoot
    {
        public string UserName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Role { get; private set; } = null!;
        public string DisplayName { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { } // for EF Core materialization

        public static User Register(string userName, Email email, string passwordHash, string? displayName = null)
        {
            Validate(userName, passwordHash);

            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash,
                Role = "User",
                DisplayName = displayName ?? string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.AddDomainEvent(new UserCreatedDomainEvent(user));

            return user;
        }

        private static void Validate(string userName, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password is required.");
        }
    }
}
