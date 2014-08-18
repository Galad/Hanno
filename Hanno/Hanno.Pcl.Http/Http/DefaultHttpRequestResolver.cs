using System;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	public class DefaultHttpRequestResolver : IHttpRequestResolver
	{
		private readonly IHttpRequestSpecificationResolver _requestDefinitionSelector;
		private readonly IHttpRequestBuilder _httpRequestBuilder;
		private readonly IHttpRequestSpecificationsTransformer _specsResolver;

		public DefaultHttpRequestResolver(
			IHttpRequestSpecificationResolver requestDefinitionSelector,
			IHttpRequestBuilder httpRequestBuilder,
			IHttpRequestSpecificationsTransformer specsResolver)
		{
			if (requestDefinitionSelector == null) throw new ArgumentNullException("requestDefinitionSelector");
			if (httpRequestBuilder == null) throw new ArgumentNullException("httpRequestBuilder");
			if (specsResolver == null) throw new ArgumentNullException("specsResolver");
			_requestDefinitionSelector = requestDefinitionSelector;
			_httpRequestBuilder = httpRequestBuilder;
			_specsResolver = specsResolver;
		}

		public async Task<IHttpRequest> ResolveHttpRequest<TQuery>(TQuery parameter) where TQuery : IAsyncParameter
		{
			var specs = _requestDefinitionSelector.ResolveHttpRequestSpecification(parameter);
			var options = await _specsResolver.TransformHttpSpecificationsToHttpBuilderOptions(specs, parameter);
			var request = options.ToRequest(_httpRequestBuilder);
			return request;
		}
	}
}