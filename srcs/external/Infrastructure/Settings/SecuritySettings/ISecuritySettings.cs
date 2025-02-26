namespace Infrastructure.Settings.SecuritySettings;

public interface ISecuritySettings {
	string        EncryptionKey                   { get; set; }
	string        Salt                            { get; set; }
	public string Issuer                          { get; set; }
	public string Audience                        { get; set; }
	public string SecretKey                       { get; set; }
	public int    ExpirationInMinutes             { get; set; }
	public int    RefreshTokenExpirationInMinutes { get; set; }
	public int    ExpiredTokenControlMinutes      { get; set; }
	public string RedisConnectionString           { get; set; }
}