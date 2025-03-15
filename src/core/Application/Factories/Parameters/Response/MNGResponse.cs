namespace Application.Factories.Parameters.Response
{
    public sealed class MNGResponseToken(
        string Jwt,
        string RefreshToken,
        string JwtExpireDate,
        string RefreshTokenExpireDate)
    {
        public string Jwt                    { get; set; } = Jwt;
        public string RefreshToken           { get; set; } = RefreshToken;
        public string JwtExpireDate          { get; set; } = JwtExpireDate;
        public string RefreshTokenExpireDate { get; set; } = RefreshTokenExpireDate;
    }
}