//using System;

//namespace Hanno.Storage
//{
//	internal class TransactionAsyncDataTableFactory : IAsyncDataTableFactory
//	{
//		private readonly Action<ITransaction> _tableCreatedCallback;
//		private readonly ITransactionalAsyncDataTableFactory _transactionTableFactory;

//		public TransactionAsyncDataTableFactory(
//			Action<ITransaction> tableCreatedCallback,
//			ITransactionalAsyncDataTableFactory transactionTableFactory)
//		{
//			if (tableCreatedCallback == null) throw new ArgumentNullException("tableCreatedCallback");
//			if (transactionTableFactory == null) throw new ArgumentNullException("transactionTableFactory");
//			_tableCreatedCallback = tableCreatedCallback;
//			_transactionTableFactory = transactionTableFactory;
//			TransactionId = Guid.NewGuid();
//		}

//		public Guid TransactionId { get; private set; }

//		public IAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>()
//		{
//			var table = _transactionTableFactory.GetAsyncTable<TKey, TValue>(TransactionId);
//			_tableCreatedCallback(table);
//			return table;
//		}

//		public void Release<TKey, TValue>(IAsyncDataTable<TKey, TValue> table)
//		{
//			var transactionTable = table as ITransactionalAsyncDataTable<TKey, TValue>;
//			if (transactionTable == null)
//			{
//				throw new InvalidOperationException("The table is not valid. It has not been created by this  instance of IAsyncTableFactory");
//			}
//			_transactionTableFactory.ReleaseAsyncTable(transactionTable);
//		}
//	}
//}