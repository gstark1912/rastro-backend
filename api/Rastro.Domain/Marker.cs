using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RastroApi.Domain.Common;

namespace Rastro.Domain;

public class Marker : IEntity, IUserOwned
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ProjectId { get; set; }
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public MarkerType Type { get; set; }
    public MarkerFrequency Frequency { get; set; }
    public float? DailyTarget { get; set; }
    public float? WeeklyTarget { get; set; }
    public int Order { get; set; }    
}

public enum MarkerType
{
    Number,
    Boolean
}

public enum MarkerFrequency
{
    Daily,
    Weekly
}
