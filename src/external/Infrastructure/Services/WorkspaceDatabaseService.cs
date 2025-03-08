using Application.Services;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Services;

public sealed class WorkspaceDatabaseService(
    IMongoClient client) : IWorkspaceDatabaseService
{
    public async Task CreateWorkspaceDatabaseAsync(string id, CancellationToken cancellationToken = default)
    {
        var database = client.GetDatabase("workspace_" + id);
        var collection = database.GetCollection<User>("test");

        await collection.InsertOneAsync(new User { Name = "test" }, cancellationToken: cancellationToken);
    }
}