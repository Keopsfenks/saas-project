using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Infrastructure.Settings.SecuritySettings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public sealed class JwtProvider(
    ISecuritySettings securitySettings) : IJwtProvider
{
    public Task<TokenDto> GenerateJwtToken(User user, List<Workspace?> workspaces, string? WorkspaceId)
    {
        List<Claim> claims = new() {
                                       new Claim("Id",         user.Id),
                                       new Claim("Name",       user.Name),
                                       new Claim("Surname",    user.Surname),
                                       new Claim("Email",      user.Email),
                                       new Claim("Workspace",  WorkspaceId ?? string.Empty),
                                       new Claim("Workspaces", JsonSerializer.Serialize(workspaces))
                                   };

        DateTime expires = DateTime.UtcNow.AddMinutes(securitySettings.ExpirationInMinutes);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.SecretKey));

        JwtSecurityToken jwtSecurityToken = new(
            issuer: securitySettings.Issuer,
            audience: securitySettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        JwtSecurityTokenHandler tokenHandler = new();

        string token = tokenHandler.WriteToken(jwtSecurityToken);

        string refreshToken = Guid.NewGuid().ToString();
        DateTime refreshExpires = DateTime.UtcNow.AddHours(securitySettings.RefreshTokenExpirationInMinutes);

        TokenDto tokenDto = new()
        {
            Token = token,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshExpires,
            ExpiryTime = expires
        };

        return Task.FromResult(tokenDto);
    }
}