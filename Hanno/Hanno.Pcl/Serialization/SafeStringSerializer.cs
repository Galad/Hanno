using System;
using System.IO;
using System.Text;

namespace Hanno.Serialization
{
	public class SafeStringSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;
		private readonly Encoding _encoding;

		public SafeStringSerializer(ISerializer innerSerializer, Encoding encoding)
		{
			if (innerSerializer == null) throw new ArgumentNullException("innerSerializer");
			if (encoding == null) throw new ArgumentNullException("encoding");
			_innerSerializer = innerSerializer;
			_encoding = encoding;
		}

		public SafeStringSerializer(ISerializer innerSerializer)
			: this(innerSerializer, Encoding.UTF8)
		{
		}

		public void Serialize<T>(T value, Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (typeof(T) == typeof(string))
			{
				var valueString = (string)(object)value;
				var bytes = _encoding.GetBytes(valueString);
				stream.Write(bytes, 0, bytes.Length);
				stream.Position = 0;
				return;
			}
			_innerSerializer.Serialize(value, stream);
		}
	}
}