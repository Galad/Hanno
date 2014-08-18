using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public interface IHttpRequestResolver
	{
		Task<IHttpRequest> ResolveHttpRequest<TQuery>(TQuery parameter) where TQuery : IAsyncParameter;
	}
}