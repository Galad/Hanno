using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanno.Http
{
	public interface IHttpRequestBuilder
	{
		IHttpRequest BuildRequest(IHttpRequestDefinition httpRequestDefinition);
	}
}
