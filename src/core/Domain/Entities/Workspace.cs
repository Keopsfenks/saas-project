using Domain.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class Workspace : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


    [BsonIgnore]
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}