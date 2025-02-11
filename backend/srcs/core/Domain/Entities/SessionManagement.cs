using Domain.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public sealed class SessionManagement : BaseEntity {

	public string Token                  { get; set; }
	public string RefreshToken           { get; set; }
	public string RefreshTokenExpiryTime { get; set; }

	public string  UserId { get; set; }
	[BsonIgnore]
	public AppUser User   { get; set; }
}