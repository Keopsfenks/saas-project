namespace Infrastructure.Settings.EmailSettings;

public interface IEmailSettings {
	public string Email               { get; set; }
	public string MailHost            { get; set; }
	public int    MailPort            { get; set; }
	public string CredentialsName     { get; set; }
	public string CredentialsPassword { get; set; }
}