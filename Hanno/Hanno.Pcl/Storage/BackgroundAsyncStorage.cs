using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Storage
{
	public class BackgroundAsyncStorage : IAsyncStorage
	{
		private readonly IAsyncStorage _innerAsyncStorage;

		public BackgroundAsyncStorage(
			IAsyncStorage innerAsyncStorage)
		{
			if (innerAsyncStorage == null) throw new ArgumentNullException("innerAsyncStorage");
			_innerAsyncStorage = innerAsyncStorage;
		}

		public async Task<TValue> Get<TKey, TValue>(TKey key, CancellationToken ct)
		{
			return await Task.Run(() => _innerAsyncStorage.Get<TKey, TValue>(key, ct));
		}

		public async Task AddOrUpdate<TKey, TValue>(TKey key, TValue value, CancellationToken ct)
		{
			await Task.Run(() => _innerAsyncStorage.AddOrUpdate(key, value, ct));
		}

		public async Task Delete<TKey, TValue>(TKey key, CancellationToken ct)
		{
			await Task.Run(() => _innerAsyncStorage.Delete<TKey, TValue>(key, ct));
		}

		public Task<bool> Contains<TKey, TValue>(TKey key, CancellationToken ct)
		{
			return Task.Run(() => _innerAsyncStorage.Contains<TKey, TValue>(key, ct));
		}
	}
}