using System.Net.Http;

namespace Hanno.Http
{
	public static class HttpRequestMethodBuilderExtensions
	{
		public static IHttpRequestBuilderOptions Post(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Post);
		}

		public static IHttpRequestBuilderOptions Get(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Get);
		}

		public static IHttpRequestBuilderOptions Put(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Put);
		}

		public static IHttpRequestBuilderOptions Trace(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Trace);
		}

		public static IHttpRequestBuilderOptions Delete(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Delete);
		}

		public static IHttpRequestBuilderOptions Options(this IHttpRequestMethodBuilder builder)
		{
			return builder.Method(HttpMethod.Options);
		}
	}
}