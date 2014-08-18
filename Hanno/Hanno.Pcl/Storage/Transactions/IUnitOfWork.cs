//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Hanno.Storage
//{
//	/// <summary>
//	/// Unit of work. Represent a  transaction
//	/// </summary>
//	public interface IUnitOfWork : IDisposable
//	{
//		/// <summary>
//		/// Get a table for the transaction
//		/// </summary>
//		/// <typeparam name="TKey"></typeparam>
//		/// <typeparam name="TValue"></typeparam>
//		/// <returns></returns>
//		IAsyncStorage Storage { get; }
//		void Commit();
//	}
//}