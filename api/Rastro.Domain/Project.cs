using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RastroApi.Domain.Common;

namespace Rastro.Domain;

[BsonIgnoreExtraElements]
public class Project : IEntity, IUserOwned
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("title")]
    public string Title { get; set; } = default!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;
}
