using Rastro.Domain;

namespace Rastro.Application.Abstractions
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
