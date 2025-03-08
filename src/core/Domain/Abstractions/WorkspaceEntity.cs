using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Abstractions;

public abstract class WorkspaceEntity : IEntity {
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string          Id           { get; set; } = ObjectId.GenerateNewId().ToString();

	public DateTimeOffset  CreateAt { get; set; }
	public DateTimeOffset? UpdateAt { get; set; }
	public DateTimeOffset? DeleteAt { get; set; }
	public bool            IsDeleted { get; set; }
}