using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Storage
{
	public class AsyncStorage : IAsyncStorage
	{
		private readonly IAsyncDataTableFactory _dataTableFactory;
		private readonly Dictionary<Type, object> _tables = new Dictionary<Type, object>();

		public AsyncStorage(
			IAsyncDataTableFactory dataTableFactory
			)
		{
			if (dataTableFactory == null) throw new ArgumentNullException("dataTableFactory");
			_dataTableFactory = dataTableFactory;
		}

		public async Task<TValue> Get<TKey, TValue>(TKey key, CancellationToken ct)
		{
			var table = GetTable<TKey, TValue>();
			return await table.Get(key);
		}

		public Task AddOrUpdate<TKey, TValue>(TKey key, TValue value, CancellationToken ct)
		{
			var table = GetTable<TKey, TValue>();
			return table.AddOrUpdate(key, value);
		}

		public Task Delete<TKey, TValue>(TKey key, CancellationToken ct)
		{
			var table = GetTable<TKey, TValue>();
			return table.Delete(key);
		}

		public Task<bool> Contains<TKey, TValue>(TKey key, CancellationToken ct)
		{
			var table = GetTable<TKey, TValue>();
			return table.Contains(key);
		}

		private IAsyncDataTable<TKey, TValue> GetTable<TKey, TValue>()
		{
			object table;
			var type = typeof(TValue);
			if (!_tables.TryGetValue(type, out table))
			{
				_tables[type] = table = _dataTableFactory.GetAsyncTable<TKey, TValue>();
			}
			return (IAsyncDataTable<TKey, TValue>)table;
		}
	}
}