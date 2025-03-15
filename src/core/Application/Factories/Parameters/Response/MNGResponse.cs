namespace Application.Factories.Parameters.Response
{
    public sealed record MNGResponseToken(
        string Jwt,
        string RefreshToken,
        string JwtExpireDate,
        string RefreshTokenExpireDate);
}