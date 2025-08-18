using Rastro.Domain.Infra;

namespace Rastro.Domain
{
    public class User : BaseEntity
    {
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string? DisplayName { get; private set; }
        public bool EmailVerified { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

        private User() { }

        public static User CreateLocal(string email, string displayName, string passwordPlaintext)
            => new()
            {
                Email = email.Trim().ToLowerInvariant(),
                DisplayName = displayName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlaintext, workFactor: 12),
                EmailVerified = false
            };

        public bool VerifyPassword(string plaintext)
            => BCrypt.Net.BCrypt.Verify(plaintext, PasswordHash);

        public void MarkEmailVerified() { EmailVerified = true; UpdatedAtUtc = DateTime.UtcNow; }
        
        public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
