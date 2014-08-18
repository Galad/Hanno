using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hanno.Storage
{
	/// <summary>
	/// Asynchronous data table
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public interface IAsyncDataTable<TKey, TValue>
	{
		Task<TValue> Get(TKey key);
		Task AddOrUpdate(TKey key, TValue value);
		Task Delete(TKey key);
		Task<KeyValuePair<TKey, TValue>[]> Query(Func<IQueryable<KeyValuePair<TKey, TValue>>, IQueryable<KeyValuePair<TKey, TValue>>> query);
		Task<bool> Contains(TKey key);
	}
}