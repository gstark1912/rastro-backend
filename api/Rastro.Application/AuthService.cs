using Rastro.Application.Abstractions;
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

        public async Task<string> LoginAsync(User request)
        {
            var user = await _repo.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash))
                throw new UnauthorizedAccessException();

            var token = _jwt.GenerateToken(user);
            return token;
        }
    }
}
