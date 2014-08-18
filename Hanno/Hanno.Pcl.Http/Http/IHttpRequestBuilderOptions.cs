using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace Hanno.Http
{
	public interface IHttpRequestBuilderOptions
	{
		IHttpRequest ToRequest(IHttpRequestBuilder httpRequestBuilder);
		IHttpRequestBuilderOptions AppendPath(string path);
		/// <summary>
		/// Add a parameter to the uri
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException">A parameter with the same key already exists</exception>
		/// <returns></returns>
		IHttpRequestBuilderOptions Parameter(string key, string value);
		/// <summary>
		/// Add a parameter to the payload
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException">A parameter with the same key already exists</exception>
		/// <returns></returns>
		IHttpRequestBuilderOptions PayloadParameter(string key, string value);
		/// <summary>
		/// Add a header
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException">A header with the same key already exists</exception>
		/// <returns></returns>
		IHttpRequestBuilderOptions Header(string key, string value);
		/// <summary>
		/// Perform action on HttpHeaders object
		/// Actions are chained
		/// </summary>
		/// <param name="setHeaders"></param>
		/// <returns></returns>
		IHttpRequestBuilderOptions Header(Action<HttpHeaders> setHeaders);
		IHttpRequestBuilderOptions Cookie(Uri uri, Cookie cookie);
	}
}