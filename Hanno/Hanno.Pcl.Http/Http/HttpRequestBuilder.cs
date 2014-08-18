using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Hanno.Http
{
	public class HttpRequestBuilder : IHttpRequestBuilder
	{
		public IHttpRequest BuildRequest(IHttpRequestDefinition httpRequestDefinition)
		{
			var requestMessage = new HttpRequestMessage(httpRequestDefinition.HttpMethod, httpRequestDefinition.Uri);
			var content = GetContent(httpRequestDefinition);
			if (content != null)
			{
				requestMessage.Content = content;
			}
			foreach (var header in httpRequestDefinition.Headers)
			{
				requestMessage.Headers.Add(header.Key, header.Value);
			}
			foreach (var headerAction in httpRequestDefinition.HeaderActions)
			{
				headerAction(requestMessage.Headers);
			}
			CookieContainer cookies = null;
			if (httpRequestDefinition.Cookies.Any())
			{
				cookies = new CookieContainer();
				foreach (var cookie in httpRequestDefinition.Cookies)
				{
					cookies.Add(cookie.Item1, cookie.Item2);
				}
			}
			return new HttpRequest(requestMessage, cookies);
		}

		private HttpContent GetContent(IHttpRequestDefinition httpRequestDefinition)
		{
			if (httpRequestDefinition.PayloadParameters.Any())
			{
				return new FormUrlEncodedContent(httpRequestDefinition.PayloadParameters);
			}
			if (httpRequestDefinition.StreamContent != null)
			{
				return new StreamContent(httpRequestDefinition.StreamContent);
			}
			if (!string.IsNullOrEmpty(httpRequestDefinition.StringContent))
			{
				return new StringContent(httpRequestDefinition.StringContent);
			}
			return null;
		}
	}
}