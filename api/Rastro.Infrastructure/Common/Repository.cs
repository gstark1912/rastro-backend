// Infrastructure/Mongo/MongoRepository.cs
using System.Linq.Expressions;
using MongoDB.Driver;
using RastroApi.Domain.Common;
using Rastro.Infrastructure.Abstractions;

namespace RastroApi.Infrastructure.Mongo;

public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _col;

    public Repository(IMongoDatabase ctx, string? collectionName = null)
    {
        var name = !string.IsNullOrWhiteSpace(collectionName)
            ? collectionName
            : GetDefaultCollectionName(typeof(T));
        _col = ctx.GetCollection<T>(name);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> filter, int skip = 0, int take = 100, CancellationToken ct = default)
        => await _col.Find(filter).Skip(skip).Limit(take).ToListAsync(ct);

    public async Task<T> InsertAsync(T entity, CancellationToken ct = default)
    {
        await _col.InsertOneAsync(entity, cancellationToken: ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        var result = await _col.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _col.DeleteOneAsync(x => x.Id == id, ct);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    private static string GetDefaultCollectionName(Type t)
    {
        // very safe, predictable, no “clever” pluralization
        // e.g. Project -> "projects", User -> "users"
        return (t.Name.EndsWith("s", StringComparison.OrdinalIgnoreCase)
            ? t.Name.ToLowerInvariant()
            : $"{t.Name.ToLowerInvariant()}s");
    }
}
