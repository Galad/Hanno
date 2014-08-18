using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Hanno.Storage
{
	//public class AsyncFromStringKeyValuePairFilesDataTable<TKey, TValue> : IAsyncDataTable<TKey, TValue>
	//{
	//	private readonly IDataTable<TKey, TValue> _synchronousTable;

	//	public AsyncFromStringKeyValuePairFilesDataTable(IDataTable<TKey, TValue> synchronousTable)
	//	{
	//		if (synchronousTable == null) throw new ArgumentNullException("synchronousTable");
	//		_synchronousTable = synchronousTable;
	//	}

	//	public Task<TValue> Get(TKey key)
	//	{
	//		return Task.Run(() => _synchronousTable.Get(key));
	//	}

	//	public Task AddOrUpdate(TKey key, TValue value)
	//	{
	//		return Task.Run(() => _synchronousTable.AddOrUpdate(key, value));
	//	}

	//	public Task Delete(TKey key)
	//	{
	//		return Task.Run(() => _synchronousTable.Delete(key));
	//	}

	//	public Task<KeyValuePair<TKey, TValue>[]> Query(Func<IQueryable<KeyValuePair<TKey, TValue>>, IQueryable<KeyValuePair<TKey, TValue>>>  query)
	//	{
	//		if (query == null) throw new ArgumentNullException("query");
	//		return Task.Run(() => query(_synchronousTable.Query()).ToArray());
	//	}

	//	public Task<bool> Contains(TKey key)
	//	{
	//		return Task.Run(() => _synchronousTable.Contains(key));
	//	}
	//}
}