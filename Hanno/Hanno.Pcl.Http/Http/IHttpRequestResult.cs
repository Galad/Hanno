using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Hanno.Http
{
	public interface IHttpRequestResult
	{
		IHttpResponseContent Content { get; }
		IEnumerable<KeyValuePair<string,IEnumerable<string>>> Headers { get; }
		string ReasonPhrase { get; }
		HttpStatusCode StatusCode { get; }
	}
}