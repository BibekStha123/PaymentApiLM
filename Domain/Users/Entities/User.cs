using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Users.Events;

namespace PaymentDetailApi.Domain.User.Entities
{
    public class User : Entity
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { }

        public static User Register(string userName, string email, string passwordHash, string? displayName = null)
        {
            Validate(userName, email, passwordHash);

            var user = new User
            {
                Id = Guid.CreateVersion7(),
                UserName = userName,
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                Role = "User",
                DisplayName = displayName ?? string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.AddDomainEvent(new UserCreatedDomainEvent(user));

            return user;
        }

        private static void Validate(string userName, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("A valid email is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password is required.");
        }
    }
}
