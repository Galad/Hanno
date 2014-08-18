using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public interface IHttpRequestSpecificationResolver
	{
		IHttpRequestSpecification ResolveHttpRequestSpecification<T>(T query) where T : IAsyncParameter;
	}
}