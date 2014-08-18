using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Http
{
	public interface IHttpResponseContent
	{
		IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }
		Task<Stream> ReadStream(CancellationToken ct);
	}
}