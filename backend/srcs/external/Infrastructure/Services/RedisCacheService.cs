using Application.Services;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Services;

public sealed class RedisCacheService(IConnectionMultiplexer redis) : ICacheService {
	private readonly IDatabase database = redis.GetDatabase();

	public T? Get<T>(string key) {
		var value = database.StringGet(key);
		if (value.HasValue)
		{
			var result = JsonConvert.DeserializeObject<T?>(value.ToString());
			return result;
		}

		return default(T?);
	}

	public void Set<T>(string key, T value, TimeSpan? expiry = null) {
		var serializedValue = JsonConvert.SerializeObject(value);
		database.StringSet(key, serializedValue, expiry);
	}

	public bool Remove(string key) {
		return database.KeyDelete(key);
	}

	public void RemoveAll() {
		var endpoints = redis.GetEndPoints();
		var server = redis.GetServer(endpoints.First());
		server.FlushDatabase();
	}
}