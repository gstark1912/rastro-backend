using MongoDB.Driver;
using RastroApi.Models;
using RastroApi.Services;

namespace RastroApi.Services;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(string id);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(string id, Item item);
    Task DeleteAsync(string id);
}

public class ItemRepository : IItemRepository
{
    private readonly IMongoCollection<Item> _items;

    public ItemRepository(IMongoDbService mongoDbService)
    {
        _items = mongoDbService.GetCollection<Item>("items");
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return await _items.Find(item => item.IsActive).ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(string id)
    {
        return await _items.Find(item => item.Id == id && item.IsActive).FirstOrDefaultAsync();
    }

    public async Task<Item> CreateAsync(Item item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        await _items.InsertOneAsync(item);
        return item;
    }

    public async Task UpdateAsync(string id, Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        await _items.ReplaceOneAsync(i => i.Id == id, item);
    }

    public async Task DeleteAsync(string id)
    {
        await _items.UpdateOneAsync(
            i => i.Id == id, 
            Builders<Item>.Update.Set(i => i.IsActive, false).Set(i => i.UpdatedAt, DateTime.UtcNow)
        );
    }
}
