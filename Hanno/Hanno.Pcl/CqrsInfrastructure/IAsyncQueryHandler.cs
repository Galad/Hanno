using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public interface IAsyncQueryHandler<in TQuery, TResult> where TQuery : IAsyncQuery<object>
	{
		Task<TResult> Execute(TQuery command);
	}
}