namespace Infrastructure.Settings.DatabaseSettings;

public interface IDatabaseSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string WorkspaceNamespace { get; set; }
}