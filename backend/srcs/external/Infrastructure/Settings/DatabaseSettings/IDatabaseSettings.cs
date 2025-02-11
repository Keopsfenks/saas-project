namespace Infrastructure.Settings.DatabaseSetting;

public interface IDatabaseSettings {
	string UserCollectionName    { get; set; }
	string SessionCollectionName { get; set; }
	string ConnectionString      { get; set; }
	string DatabaseName          { get; set; }
}