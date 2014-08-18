using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public interface IHttpRequestSpecificationsTransformer
	{
		Task<IHttpRequestBuilderOptions> TransformHttpSpecificationsToHttpBuilderOptions<T>
			(IHttpRequestSpecification specifications, T parameter) where T : IAsyncParameter;
	}
}