// Application/Abstractions/IRepository.cs
using System.Linq.Expressions;
using RastroApi.Domain.Common;

namespace Rastro.Infrastructure.Abstractions;

public interface IRepository<T> where T : IEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> filter, int skip = 0, int take = 100, CancellationToken ct = default);
    Task<T> InsertAsync(T entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(T entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
