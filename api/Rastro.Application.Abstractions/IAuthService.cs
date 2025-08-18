using Rastro.Application.Contracts.Auth;
using Rastro.Domain;

namespace Rastro.Application.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
