using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	/// <summary>
	/// Build http request options for the query.
	/// </summary>
	/// <typeparam name="TQuery">Query type</typeparam>
	public interface IBuildHttpRequest<in TQuery> where TQuery : IAsyncParameter
	{
		/// <summary>
		/// Buid the http request options asynchronously, and the return the builder
		/// </summary>
		/// <param name="options">The builder containing the http request options</param>
		/// <param name="query">The query</param>
		/// <returns>The builder containing the http request options</returns>
		Task<IHttpRequestBuilderOptions> Build(IHttpRequestBuilderOptions options, TQuery query);
	}
}