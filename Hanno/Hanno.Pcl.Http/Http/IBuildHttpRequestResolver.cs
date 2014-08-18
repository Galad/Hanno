using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public interface IBuildHttpRequestResolver
	{
		IBuildHttpRequest<T> ResolveBuildHttpRequest<T>(T parameter) where T : IAsyncParameter;
	}
}