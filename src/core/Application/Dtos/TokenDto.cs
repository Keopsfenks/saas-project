using Domain.Entities;

namespace Application.Dtos;

public sealed class TokenDto
{
    public TokenDto(string token, string refreshToken, DateTime RefreshTokenExpiryTime, DateTime ExpiryTime)
    {
        Token = token;
        RefreshToken = refreshToken;
        this.RefreshTokenExpiryTime = RefreshTokenExpiryTime;
        this.ExpiryTime = ExpiryTime;
    }
    public string          Token                  { get; set; }
    public string          RefreshToken           { get; set; }
    public DateTime        RefreshTokenExpiryTime { get; set; }
    public DateTime        ExpiryTime             { get; set; }
    public DateTimeOffset  CreatedAt              { get; set; }
    public DateTimeOffset? UpdatedAt              { get; set; }
}