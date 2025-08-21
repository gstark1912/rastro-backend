// Application/Common/UserScopedCrudService.cs
using System.Linq.Expressions;
using RastroApi.Domain.Common;
using Rastro.Application.Abstractions;
using Rastro.Infrastructure.Abstractions;

namespace Rastro.Application;

public class UserScopedCrudService<T> : IUserScopedCrudService<T> where T : class, IEntity, IUserOwned, new()
{
    private readonly IRepository<T> _repo;

    public UserScopedCrudService(IRepository<T> repo)
    {
        _repo = repo;
    }

    public async Task<T?> GetAsync(string id, string userId, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity?.UserId == userId ? entity : null;
    }

    public Task<IReadOnlyList<T>> ListAsync(string userId, int skip = 0, int take = 100, CancellationToken ct = default)
        => _repo.FindAsync(x => x.UserId == userId, skip, take, ct);

    public async Task<T> CreateAsync(T entity, string userId, CancellationToken ct = default)
    {
        entity.Id = entity.Id ?? string.Empty; // let Mongo set ObjectId
        entity.UserId = userId;
        return await _repo.InsertAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(T entity, string userId, CancellationToken ct = default)
    {
        // enforce ownership
        var existing = await _repo.GetByIdAsync(entity.Id, ct);
        if (existing == null || existing.UserId != userId) return false;

        entity.UserId = userId; // never allow changing ownership
        return await _repo.UpdateAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(string id, string userId, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null || existing.UserId != userId) return false;

        return await _repo.DeleteAsync(id, ct);
    }
}
