using MongoDB.Driver;
using Rastro.Domain;
using Rastro.Infrastructure.Abstractions;

namespace Rastro.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<User>("users");
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            (await _collection.FindAsync(u => u.Email == email)).FirstOrDefault();

        public Task CreateAsync(User user) =>
            _collection.InsertOneAsync(user);
    }
}
