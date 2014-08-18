using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Serialization;

namespace Hanno.Http
{
	public class DeserializerHttpRequestResultReader : IHttpRequestResultReader
	{
		private readonly IDeserializer _deserializer;

		public DeserializerHttpRequestResultReader(IDeserializer deserializer)
		{
			if (deserializer == null) throw new ArgumentNullException("deserializer");
			_deserializer = deserializer;
		}

		public async Task<T> Read<T>(IHttpRequestResult result, CancellationToken ct)
		{
			using (var stream = await result.Content.ReadStream(ct))
			{
				return _deserializer.Deserialize<T>(stream);
			}
		}
	}
}
