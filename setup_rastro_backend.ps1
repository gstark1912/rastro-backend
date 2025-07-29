# Script to create a .NET 6 Web API project for Rastro
# Run this from the rastro-backend directory

Write-Host "Creating Rastro API project with MongoDB support..." -ForegroundColor Green

# Check if we're in the right directory
if (-not (Test-Path "api")) {
    Write-Host "Error: 'api' folder not found. Make sure you're in the rastro-backend directory." -ForegroundColor Red
    exit 1
}

# Navigate to api folder
Set-Location api

# Create new Web API project
Write-Host "Creating .NET 6 Web API project..." -ForegroundColor Yellow
dotnet new webapi -n RastroApi --framework net6.0

# Navigate to project folder
Set-Location RastroApi

Write-Host "Adding MongoDB driver and related packages..." -ForegroundColor Yellow

# Add MongoDB driver
dotnet add package MongoDB.Driver --version 2.25.0

# Add configuration extensions
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions --version 6.0.0

# Add Swagger for API documentation (usually included, but ensure it's there)
dotnet add package Swashbuckle.AspNetCore --version 6.5.0

Write-Host "Creating MongoDB models and services..." -ForegroundColor Yellow

# Create Models folder and sample model
New-Item -ItemType Directory -Name "Models" -Force
$sampleModelContent = @"
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RastroApi.Models;

public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Sample entity - replace with your actual entities
public class Item : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
"@

$sampleModelContent | Out-File -FilePath "Models/BaseEntity.cs" -Encoding UTF8

# Create database configuration model
$dbConfigContent = @"
namespace RastroApi.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
"@

$dbConfigContent | Out-File -FilePath "Models/MongoDbSettings.cs" -Encoding UTF8

# Create Services folder and MongoDB service
New-Item -ItemType Directory -Name "Services" -Force
$mongoServiceContent = @"
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
"@

$mongoServiceContent | Out-File -FilePath "Services/MongoDbService.cs" -Encoding UTF8

# Create a sample repository
$repositoryContent = @"
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
"@

$repositoryContent | Out-File -FilePath "Services/ItemRepository.cs" -Encoding UTF8

# Create Controllers folder and sample controller
New-Item -ItemType Directory -Name "Controllers" -Force
$controllerContent = @"
using Microsoft.AspNetCore.Mvc;
using RastroApi.Models;
using RastroApi.Services;

namespace RastroApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepository;

    public ItemsController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Item>>> GetAll()
    {
        var items = await _itemRepository.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetById(string id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        var createdItem = await _itemRepository.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Item item)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        item.Id = id;
        await _itemRepository.UpdateAsync(id, item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        await _itemRepository.DeleteAsync(id);
        return NoContent();
    }
}
"@

$controllerContent | Out-File -FilePath "Controllers/ItemsController.cs" -Encoding UTF8

# Update Program.cs to configure MongoDB
$programContent = @"
using RastroApi.Models;
using RastroApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
"@

$programContent | Out-File -FilePath "Program.cs" -Encoding UTF8

# Update appsettings.json
$appSettingsContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password@localhost:27017",
    "DatabaseName": "rastro"
  }
}
"@

$appSettingsContent | Out-File -FilePath "appsettings.json" -Encoding UTF8

# Update appsettings.Development.json
$appSettingsDevContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Extensions.Hosting": "Information"
    }
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password@localhost:27017",
    "DatabaseName": "rastro_development"
  }
}
"@

$appSettingsDevContent | Out-File -FilePath "appsettings.Development.json" -Encoding UTF8

# Remove default WeatherForecast files
Remove-Item "Controllers/WeatherForecastController.cs" -ErrorAction SilentlyContinue
Remove-Item "WeatherForecast.cs" -ErrorAction SilentlyContinue

