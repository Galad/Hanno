using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Storage
{
	/// <summary>
	/// Asynchrous storage.
	/// </summary>
	public interface IAsyncStorage
	{
		Task<TValue> Get<TKey, TValue>(TKey key, CancellationToken ct);
		Task AddOrUpdate<TKey, TValue>(TKey key, TValue value, CancellationToken ct);
		Task Delete<TKey, TValue>(TKey key, CancellationToken ct);
		Task<bool> Contains<TKey, TValue>(TKey key, CancellationToken ct);
	}
}