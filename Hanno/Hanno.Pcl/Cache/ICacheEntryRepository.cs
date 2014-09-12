using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Storage;

namespace Hanno.Cache
{
	public interface ICacheEntryRepository
	{
		Task<CacheEntry<T>> Get<T>(string cacheKey, IDictionary<string, string> attributes);
		Task AddOrUpdate<T>(CacheEntry<T> entry);
		Task Remove(Guid id);
	}
}