using Domain.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class Session : BaseEntity {

	public string   Token                  { get; set; } = string.Empty;
	public string   RefreshToken           { get; set; } = string.Empty;
	public DateTime RefreshTokenExpiryTime { get; set; }
	public DateTime ExpiryTime             { get; set; }

	public string  UserId { get; set; } = string.Empty;
	[BsonIgnore]
	public User User   { get; set; } = null!;
}