//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Hanno.Storage
//{
//	public interface ITransactionalAsyncDataTable<TKey, TValue> : ITransaction, IAsyncDataTable<TKey, TValue>
//	{
//	}

//	public class TransactionalAsyncDataTable<TKey, TValue> : ITransactionalAsyncDataTable<TKey, TValue>
//	{
//		private readonly IAsyncDataTable<TKey, TValue> _tableForTransaction;
//		private readonly IAsyncDataTable<TKey, TValue> _innerTable;
//		private bool _isDisposed = false;
//		public TransactionalAsyncDataTable(
//			IAsyncDataTable<TKey, TValue> tableForTransaction,
//			IAsyncDataTable<TKey, TValue> innerTable)
//		{
//			_tableForTransaction = tableForTransaction;
//			_innerTable = innerTable;
//		}

//		public void Dispose()
//		{
//			throw new NotImplementedException();
//		}

//		public void Begin()
//		{
//			if (_isDisposed)
//			{
//				throw new InvalidOperationException("Transaction is disposed");
//			}
//		}

//		public void Commit()
//		{
//			throw new NotImplementedException();
//		}

//		public void Rollback()
//		{
//			throw new NotImplementedException();
//		}

//		public Task<TValue> Get(TKey key)
//		{
//			_innerTable.Get(key);
//			throw new NotImplementedException();
//		}

//		public Task AddOrUpdate(TKey key, TValue value)
//		{
//			throw new NotImplementedException();
//		}

//		public Task Delete(TKey key)
//		{
//			throw new NotImplementedException();
//		}

//		public Task<KeyValuePair<TKey, TValue>[]> Query(Func<IQueryable<KeyValuePair<TKey, TValue>>, IQueryable<KeyValuePair<TKey, TValue>>> query)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}