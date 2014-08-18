using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Http
{
	public interface IHttpRequestResultReader
	{
		Task<T> Read<T>(IHttpRequestResult result, CancellationToken ct);
	}
}
