using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Hanno.Http
{
	public class HttpRequestResult : IHttpRequestResult
	{
		private readonly HttpResponseMessage _result;
		private readonly HttpResponseContent _responseContent;

		public HttpRequestResult(HttpResponseMessage result)
		{
			if (result == null) throw new ArgumentNullException("result");
			_result = result;
			_responseContent = new HttpResponseContent(_result.Content);
		}

		public IHttpResponseContent Content { get { return _responseContent; } }
		public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get { return _result.Headers; } }
		public string ReasonPhrase { get { return _result.ReasonPhrase; } }
		public HttpStatusCode StatusCode { get { return (HttpStatusCode) (int) _result.StatusCode; } }
	}
}