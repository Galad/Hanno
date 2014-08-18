using System;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public class HttpRequestSpecificationsTransformer : IHttpRequestSpecificationsTransformer
	{
		private readonly IHttpRequestBuilderFactory _httpRequestBuilderFactory;
		private readonly IBuildHttpRequestResolver _buildHttpRequestResolver;

		public HttpRequestSpecificationsTransformer(
			IHttpRequestBuilderFactory httpRequestBuilderFactory,
			IBuildHttpRequestResolver buildHttpRequestResolver)
		{
			if (httpRequestBuilderFactory == null) throw new ArgumentNullException("httpRequestBuilderFactory");
			if (buildHttpRequestResolver == null) throw new ArgumentNullException("buildHttpRequestResolver");
			_httpRequestBuilderFactory = httpRequestBuilderFactory;
			_buildHttpRequestResolver = buildHttpRequestResolver;
		}

		public Task<IHttpRequestBuilderOptions> TransformHttpSpecificationsToHttpBuilderOptions<T>(IHttpRequestSpecification specifications, T parameter) where T : IAsyncParameter
		{
			var builderOptions = specifications.CreateSpecifiation(_httpRequestBuilderFactory);
			var buildHttpRequest = _buildHttpRequestResolver.ResolveBuildHttpRequest(parameter);
			return buildHttpRequest.Build(builderOptions, parameter);
		}
	}
}