using System;

namespace Hanno.Http
{
	public sealed class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
	{
		public IHttpRequestMethodBuilder CreateRequestBuilder(Uri uri)
		{
			return new HttpRequestBuilderOptions(uri);
		}
	}
}