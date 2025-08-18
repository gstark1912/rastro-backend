using Rastro.Application.Abstractions;
using Rastro.Application.Contracts.Auth;
using Rastro.Domain;
using Rastro.Infrastructure.Abstractions;

namespace Rastro.Application
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwt;

        public AuthService(IUserRepository repo, IJwtService jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 1) Normalize & validate
            var email = User.NormalizeEmail(request.Email);
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required.");

            // 2) Uniqueness
            var exists = await _repo.GetByEmailAsync(email);
            if (exists is not null)
                throw new InvalidOperationException("Email is already registered.");

            // 3) Create & persist
            var user = User.CreateLocal(email, request.DisplayName, request.Password);
            await _repo.CreateAsync(user);

            // 4) JWT
            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var email = User.NormalizeEmail(request.Email);
            var user = await _repo.GetByEmailAsync(email);

            // Verify plaintext vs stored hash
            if (user is null || !user.VerifyPassword(request.Password) || !user.IsActive)
                throw new UnauthorizedAccessException();

            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token);
        }
    }
}
