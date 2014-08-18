using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Http
{
	public class HttpResponseContent : IHttpResponseContent
	{
		private readonly HttpContent _httpContent;

		public HttpResponseContent(HttpContent httpContent)
		{
			if (httpContent == null) throw new ArgumentNullException("httpContent");
			_httpContent = httpContent;
		}

		public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get { return _httpContent.Headers; } }

		public async Task<Stream> ReadStream(CancellationToken ct)
		{
			return await _httpContent.ReadAsStreamAsync();
		}
	}
}