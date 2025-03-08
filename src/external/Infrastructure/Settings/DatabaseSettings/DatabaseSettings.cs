namespace Infrastructure.Settings.DatabaseSetting;

public sealed class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string WorkspaceNamespace { get; set; }
}