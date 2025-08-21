// Application/Abstractions/IUserScopedService.cs
using System.Linq.Expressions;
using RastroApi.Domain.Common;

namespace Rastro.Application.Abstractions;

public interface IUserScopedCrudService<T> where T : IEntity, IUserOwned
{
    Task<T?> GetAsync(string id, string userId, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(string userId, int skip = 0, int take = 100, CancellationToken ct = default);
    Task<T> CreateAsync(T entity, string userId, CancellationToken ct = default);
    Task<bool> UpdateAsync(T entity, string userId, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, string userId, CancellationToken ct = default);
}
