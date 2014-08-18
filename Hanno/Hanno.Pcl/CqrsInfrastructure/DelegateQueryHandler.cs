using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public abstract class DelegateQueryHandler<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult> where TQuery : IAsyncQuery<object>
	{
		protected abstract TResult ExecuteOverride(TQuery command);
		public virtual Task<TResult> Execute(TQuery command)
		{
			var result = ExecuteOverride(command);
			return Task.FromResult(result);
		}
	}
}