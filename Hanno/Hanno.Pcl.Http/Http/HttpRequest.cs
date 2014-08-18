using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Http
{
	public class HttpRequest : IHttpRequest
	{
		private readonly HttpRequestMessage _requestMessage;
		private readonly CookieContainer _cookies;

		public HttpRequest(HttpRequestMessage requestMessage, CookieContainer cookies = null)
		{
			if (requestMessage == null) throw new ArgumentNullException("requestMessage");
			_requestMessage = requestMessage;
			_cookies = cookies;
		}

		public async Task<IHttpRequestResult> Execute(CancellationToken cancellationToken)
		{
			var handler = new HttpClientHandler();
			if (_cookies != null)
			{
				handler.UseCookies = true;
				handler.CookieContainer = _cookies;
			}
			var messageHandler = new HttpMessageInvoker(handler, true);
			var result = await messageHandler.SendAsync(_requestMessage, cancellationToken);
			try
			{
				result.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException ex)
			{
				throw new HttpRequestSuccessException((HttpStatusCode)(int)result.StatusCode, ex.Message, ex);
			}
			return new HttpRequestResult(result);
		}
	}
}