using Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public sealed class MemoryCacheService(
	IMemoryCache cache) : ICacheService {
	public T? Get<T>(string key) {
		var result = cache.TryGetValue<T>(key, out var value);

		return value;
	}
	public void Set<T>(string key, T value, TimeSpan? expiry = null) {
		Console.WriteLine("test cache" + cache.GetCurrentStatistics());

		var cacheEntryOptions = new MemoryCacheEntryOptions
								{
									AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromHours(1),
								};

		cache.Set<T>(key, value, cacheEntryOptions);
	}
	public bool Remove(string key) {
		cache.Remove(key);

		return true;
	}
	public void RemoveAll() {
		cache.Dispose();
	}
}