using System;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	/// <summary>
	/// Execute a query and return the result
	/// </summary>
	public interface IAsyncQueryBus
	{
		Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>;
	}
}