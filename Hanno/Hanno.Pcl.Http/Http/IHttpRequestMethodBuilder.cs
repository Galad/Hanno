using System.Net.Http;

namespace Hanno.Http
{
	public interface IHttpRequestMethodBuilder
	{
		IHttpRequestBuilderOptions Method(HttpMethod method);
	}
}