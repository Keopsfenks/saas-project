namespace Domain.Dtos;

public sealed class TokenDto {
	public string   Token                  { get; set; }
	public string   RefreshToken           { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
	public DateTime ExpiryTime             { get; set; }
}