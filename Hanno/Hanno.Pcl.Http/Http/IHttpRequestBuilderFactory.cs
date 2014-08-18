using System;

namespace Hanno.Http
{
	public interface IHttpRequestBuilderFactory
	{
		IHttpRequestMethodBuilder CreateRequestBuilder(Uri uri);
	}
}