using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hanno.Http
{
	/// <summary>
	/// Thrown when an http request does not return a success error code
	/// </summary>
	public class HttpRequestSuccessException : Exception
	{
		public HttpStatusCode StatusCode { get; private set; }

		public HttpRequestSuccessException(HttpStatusCode statusCode, string message = null, Exception innerException = null)
			: base(message, innerException)
		{
			StatusCode = statusCode;
		}
	}
}
