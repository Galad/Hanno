using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public abstract class BuildSynchronousHttpRequest<TQuery> : IBuildHttpRequest<TQuery>
		where TQuery : IAsyncParameter
	{
		public Task<IHttpRequestBuilderOptions> Build(IHttpRequestBuilderOptions options, TQuery query)
		{
			return Task.FromResult(BuildSynchronously(options, query));
		}

		protected abstract IHttpRequestBuilderOptions BuildSynchronously(IHttpRequestBuilderOptions options, TQuery query);
	}
}
