//using System;
//using System.IO;

//namespace Hanno.Storage
//{
//	public interface ITransactionalAsyncDataTableFactory
//	{
//		ITransactionalAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>(Guid transactionId);
//		void ReleaseAsyncTable<TKey, TValue>(ITransactionalAsyncDataTable<TKey, TValue> table);
//	}

//	public class TransactionalAsyncDataTableFactory : ITransactionalAsyncDataTableFactory
//	{
//		private readonly string _dbPath;
//		private readonly string _transactionsPath;
//		private readonly Func<string, object> _tableFactory;

//		public TransactionalAsyncDataTableFactory(string dbPath, string transactionsPath, Func<string, object> tableFactory)
//		{
//			_dbPath = dbPath;
//			_transactionsPath = transactionsPath;
//			_tableFactory = tableFactory;
//		}

//		public ITransactionalAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>(Guid transactionId)
//		{
//			var dbPath = Path.Combine(_dbPath, string.Concat(_transactionsPath, "_", transactionId.ToString()));
//			return new TransactionalAsyncDataTable<TKey, TValue>(
//				GetAsyncTable<TKey, TValue>(dbPath),
//				GetAsyncTable<TKey, TValue>(_dbPath));
//		}

//		private IAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>(string path)
//		{
//			return (IAsyncDataTable<TKey, TValue>)(_tableFactory(path));
//		}

//		public void ReleaseAsyncTable<TKey, TValue>(ITransactionalAsyncDataTable<TKey, TValue> table)
//		{
//			if (table == null) throw new ArgumentNullException("table");
//			table.Dispose();
//		}
//	}
//}