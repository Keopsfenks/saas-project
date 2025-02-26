using Domain.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class Workspace : BaseEntity {
	public string Title       { get; set; }
	public string Description { get; set; }


	[BsonIgnore]
	public User   User   { get; set; }
	public string UserId { get; set; }
}