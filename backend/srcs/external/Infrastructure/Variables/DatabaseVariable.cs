using MongoDB.Driver;

namespace Infrastructure.Variables;

public class DatabaseVariable : IDatabaseVariable {
	public IMongoClient Client   { get; set; }
	public string?       Database { get; set; }
}