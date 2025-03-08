namespace Application.Dtos;

public sealed class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime ExpiryTime { get; set; }
}