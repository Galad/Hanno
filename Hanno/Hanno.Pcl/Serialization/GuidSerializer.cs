using System;
using System.IO;
using System.Text;

namespace Hanno.Serialization
{
	public class GuidDeserializer : ParsingDeserializer<Guid>
	{
		public GuidDeserializer(
			IDeserializer stringDeserializer,
			IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, Guid.Parse)
		{
		}
	}

	public class GuidSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public GuidSerializer(ISerializer innerSerializer)
		{
			if (innerSerializer == null) throw new ArgumentNullException("innerSerializer");
			_innerSerializer = innerSerializer;
		}

		public void Serialize<T>(T value, Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (typeof(T) == typeof(Guid))
			{
				var valueString = value.ToString();
				var bytes = Encoding.UTF8.GetBytes(valueString);
				stream.Write(bytes, 0, bytes.Length);
				stream.Position = 0;
				return;
			}
			_innerSerializer.Serialize(value, stream);
		}
	}
}