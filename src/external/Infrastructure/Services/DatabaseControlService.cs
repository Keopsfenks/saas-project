using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Infrastructure.Services;

public class DatabaseControlService(IMongoClient client, ILogger<DatabaseControlService> logger) : BackgroundService {
    private readonly int    Interval     = 7;
    private readonly string DatabaseName = "ProjectDb";
	private readonly DateTime OneMonthAgo = DateTime.UtcNow.AddMonths(-1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
       logger.LogInformation("DatabaseControlOperations Service Started.");

       while (!stoppingToken.IsCancellationRequested) {
          try {
             var databaseList = await client.ListDatabasesAsync(stoppingToken);
             var databases    = await databaseList.ToListAsync(stoppingToken);

             await CheckAndDeleteWorkspaces(stoppingToken);

             var targetDatabases = databases
                             .Select(db => db["name"].AsString)
                             .Where(name => name == DatabaseName || name.StartsWith("workspace_"))
                             .ToList();

             foreach (var dbname in targetDatabases) {
                var database = client.GetDatabase(dbname);
                var collections = await (await database.ListCollectionNamesAsync(cancellationToken: stoppingToken)).ToListAsync(stoppingToken);

                foreach(var collectionName in collections) {
                   logger.LogInformation($"Database: {dbname}, Collection: {collectionName}");

                   var collection = database.GetCollection<BsonDocument>(collectionName);


                   var deleteFilter = Builders<BsonDocument>.Filter.And(
                      Builders<BsonDocument>.Filter.Eq("IsDeleted", true),
                      Builders<BsonDocument>.Filter.Lt("UpdateAt", OneMonthAgo)
                   );

                   var deleteResult = await collection.DeleteManyAsync(deleteFilter, stoppingToken);

                   if (deleteResult.DeletedCount > 0)
                      logger.LogInformation(
                         $"Deleted {deleteResult.DeletedCount} documents from {collectionName} collection.");
                }
             }
          }
          catch (Exception e) {
             logger.LogError(e, "Database control operation failed");
          }

          await Task.Delay(TimeSpan.FromDays(Interval), stoppingToken);
       }
    }

    private async Task CheckAndDeleteWorkspaces(CancellationToken stoppingToken) {
        try {
            var projectDb = client.GetDatabase(DatabaseName);
            var workspacesCollection = projectDb.GetCollection<BsonDocument>("Workspaces");

			var filter = Builders<BsonDocument>.Filter.And(
				Builders<BsonDocument>.Filter.Eq("IsDeleted", true),
				Builders<BsonDocument>.Filter.Lt("UpdateAt", OneMonthAgo)
			);
            var deletedWorkspaces = await workspacesCollection.Find(filter).ToListAsync(stoppingToken);

            foreach (var workspace in deletedWorkspaces) {
                if (workspace.TryGetValue("_id", out BsonValue workspaceIdValue)) {
                    string workspaceId     = workspaceIdValue.ToString()!;
                    string workspaceDbName = $"workspace_{workspaceId}";

                    await client.DropDatabaseAsync(workspaceDbName, stoppingToken);
                    logger.LogInformation($"Dropped database: {workspaceDbName}");
                }
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Failed to check and delete workspace databases");
        }
    }
}