namespace Infrastructure.Settings.DatabaseSetting;

public sealed class DatabaseSettings : IDatabaseSettings {
	public string UserCollectionName    { get; set; }
	public string SessionCollectionName { get; set; }
	public string ConnectionString   { get; set; }
	public string DatabaseName       { get; set; }
}