using Domain.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class Session : BaseEntity {

	public string   Token                  { get; set; }
	public string   RefreshToken           { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
	public DateTime ExpiryTime             { get; set; }

	public string  UserId { get; set; }
	[BsonIgnore]
	public User User   { get; set; }
}