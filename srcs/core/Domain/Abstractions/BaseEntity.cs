using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Abstractions;

public abstract class BaseEntity : IEntity {
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string          Id           { get; set; }

	public DateTimeOffset  CreateAt     { get; set; }
	public DateTimeOffset? UpdateAt     { get; set; }
	public bool            IsDeleted    { get; set; }

}