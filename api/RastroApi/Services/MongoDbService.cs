using MongoDB.Driver;
using RastroApi.Models;
using Microsoft.Extensions.Options;

namespace RastroApi.Services;

public interface IMongoDbService
{
    IMongoDatabase Database { get; }
    IMongoCollection<T> GetCollection<T>(string collectionName);
}

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoDatabase Database => _database;

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
