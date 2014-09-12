using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Cache
{
	public interface ICacheService
	{
		/// <summary>
		/// Execute a task. If the result is in the cache, it returns the cached value. 
		/// If not, or if the cached value is older than the specified maxAge, the task is executed and its value is cached.
		/// </summary>
		/// <typeparam name="T">The type of the returned value</typeparam>
		/// <param name="ct"></param>
		/// <param name="cacheKey">The unique key identifing the cache</param>
		/// <param name="execute">A Func returning the value if the value is not cached.</param>
		/// <param name="maxAge">The maximum age for this value.</param>
		/// <param name="attributes">A dictionary containing attributes for the cache entry.</param>
		/// <returns></returns>
		Task<T> ExecuteWithCache<T>(CancellationToken ct, string cacheKey, Func<Task<T>> execute, TimeSpan maxAge, IDictionary<string, string> attributes = null);

		/// <summary>
		/// Invalidate the cache for the specified key.
		/// </summary>
		/// <param name="ct"></param>
		/// <param name="cacheKey">The unique key identifing the cache</param>
		/// <param name="attributes">A dictionary containing attributes for the cache entry.</param>
		/// <returns></returns>
		Task Invalidate<T>(CancellationToken ct, string cacheKey, IDictionary<string, string> attributes = null);

		/// <summary>
		/// Invalidate the cache for the specified key for all values older than the specified minimum age.
		/// </summary>
		/// <param name="ct"></param>
		/// <param name="cacheKey">The unique key identifing the cache</param>
		/// <param name="minAge">Minimum age to invalidate</param>
		/// <param name="attributes">A dictionary containing attributes for the cache entry.</param>
		/// <returns></returns>
		Task Invalidate<T>(CancellationToken ct, string cacheKey, TimeSpan minAge, IDictionary<string, string> attributes = null);
	}
}