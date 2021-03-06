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

}