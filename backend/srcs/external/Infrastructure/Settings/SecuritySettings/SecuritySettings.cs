namespace Infrastructure.Settings.SecuritySettings;

public sealed class SecuritySettings : ISecuritySettings {
	public string HashAlgorithmKey { get; set; }
}