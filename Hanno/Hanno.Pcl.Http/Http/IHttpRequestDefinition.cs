using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Hanno.Http
{
	public interface IHttpRequestDefinition
	{
		HttpMethod HttpMethod { get; }
		Uri Uri { get; }
		IEnumerable<KeyValuePair<string, string>> Headers { get; }
		IEnumerable<Tuple<Uri, Cookie>> Cookies { get; }
		IEnumerable<KeyValuePair<string, string>> PayloadParameters { get; }
		Stream StreamContent { get; }
		string StringContent { get; }
		IEnumerable<Action<HttpHeaders>> HeaderActions { get; }
	}
}
