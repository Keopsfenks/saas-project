using MongoDB.Driver;

namespace Infrastructure.Variables;

public interface IDatabaseVariable {
	IMongoClient Client   { get; set; }
	string?       Database { get; set; }
}