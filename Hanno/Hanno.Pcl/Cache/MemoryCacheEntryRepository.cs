using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Storage;

namespace Hanno.Cache
{
	public class MemoryCacheEntryRepository : ICacheEntryRepository
	{
		private readonly Dictionary<Guid, object> _cachedValues = new Dictionary<Guid, object>();

		public async Task<CacheEntry<T>> Get<T>(CancellationToken ct, string cacheKey, IDictionary<string, string> attributes)
		{
			var orderedAttributes = attributes.OrderBy(a => a.Key);
			CacheEntry<T> value;
			lock (_cachedValues)
			{
				value = _cachedValues.Values.OfType<CacheEntry<T>>()
				                     .FirstOrDefault(c => c.CacheKey == cacheKey &&
				                                          c.Attributes.OrderBy(a => a.Key).SequenceEqual(orderedAttributes));

			}
			return value;
		}

		public Task AddOrUpdate<T>(CancellationToken ct, CacheEntry<T> entry)
		{
			lock (_cachedValues)
			{
				_cachedValues[entry.Id] = entry;
			}
			return Task.FromResult(true);
		}

		public Task Remove<T>(CancellationToken ct, string cacheKey, Guid id)
		{
			lock (_cachedValues)
			{
				_cachedValues.Remove(id);
			}
			return Task.FromResult(true);
		}
	}

	public class AsyncStorageCacheEntryRepository : ICacheEntryRepository
	{
		private readonly IAsyncStorage _asyncStorage;

		public AsyncStorageCacheEntryRepository(IAsyncStorage asyncStorage)
		{
			if (asyncStorage == null) throw new ArgumentNullException("asyncStorage");
			_asyncStorage = asyncStorage;
		}

		public async Task<CacheEntry<T>> Get<T>(CancellationToken ct, string cacheKey, IDictionary<string, string> attributes)
		{
			var value = await _asyncStorage.Get<string, List<CacheEntry<T>>>(cacheKey, ct);
			if (value == null)
			{
				return null;
			}
			var orderedAttributes = attributes.OrderBy(a => a.Key);
			return value.FirstOrDefault(c => c.Attributes.OrderBy(a => a.Key).SequenceEqual(orderedAttributes));
		}

		public async Task AddOrUpdate<T>(CancellationToken ct, CacheEntry<T> entry)
		{
			var value = await _asyncStorage.Get<string, List<CacheEntry<T>>>(entry.CacheKey, ct);
			if (value == null)
			{
				value = new List<CacheEntry<T>>() {entry};
				await _asyncStorage.AddOrUpdate(entry.CacheKey, value, ct);
				return;
			}
			var orderedAttributes = entry.Attributes.OrderBy(a => a.Key);
			value.RemoveAll(c => c.Attributes.OrderBy(a => a.Key).SequenceEqual(orderedAttributes));
			value.Add(entry);
			await _asyncStorage.AddOrUpdate(entry.CacheKey, value, ct);
		}

		public async Task Remove<T>(CancellationToken ct, string cacheKey, Guid id)
		{
			var value = await _asyncStorage.Get<string, List<CacheEntry<T>>>(cacheKey, ct);
			if (value == null)
			{
				return;
			}
			value.RemoveAll(c => c.Id == id);
			await _asyncStorage.AddOrUpdate(cacheKey, value, ct);
		}
	}
}