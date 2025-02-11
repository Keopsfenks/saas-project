namespace Infrastructure.Settings.SecuritySettings;

public interface ISecuritySettings {
	string HashAlgorithmKey { get; set; }
}