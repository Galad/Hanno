using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Storage;

namespace Hanno.Cache
{
	public interface ICacheEntryRepository
	{
		Task<CacheEntry<T>> Get<T>(CancellationToken ct, string cacheKey, IDictionary<string, string> attributes);
		Task AddOrUpdate<T>(CancellationToken ct, CacheEntry<T> entry);
		Task Remove<T>(CancellationToken ct, string cacheKey, Guid id);
	}
}