//using System;
//using System.Collections.Generic;

//namespace Hanno.Storage
//{
//	public class UnitOfWork : IUnitOfWork
//	{
//		private readonly IAsyncStorage _storage;
//		private readonly IList<ITransaction> _transactions;

//		public UnitOfWork(ITransactionalAsyncDataTableFactory transactionalAsyncDataTableFactory)
//		{
//			_storage = new AsyncStorage(new TransactionAsyncDataTableFactory(AddTransaction, transactionalAsyncDataTableFactory));
//			_transactions = new List<ITransaction>();
//		}

//		private void AddTransaction(ITransaction transaction)
//		{
//			_transactions.Add(transaction);
//		}

//		public IAsyncStorage Storage { get { return _storage; } }

//		public void Commit()
//		{
//			try
//			{
//				foreach (var transaction in _transactions)
//				{
//					transaction.Commit();
//				}
//			}
//			catch (Exception)
//			{
//				foreach (var transaction in _transactions)
//				{
//					transaction.Rollback();
//				}
//			}
//			finally
//			{
//				foreach (var transaction in _transactions)
//				{
//					transaction.Dispose();
//				}
//			}
//		}


//		public void Dispose()
//		{
//			try
//			{
//				foreach (var transaction in _transactions)
//				{
//					transaction.Rollback();
//				}
//			}
//			finally
//			{
//				foreach (var transaction in _transactions)
//				{
//					transaction.Dispose();
//				}
//			}
//		}
//	}
//}