using Rastro.Domain;

namespace Rastro.Infrastructure.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
    }
}
