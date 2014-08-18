using System;
using System.IO;
using System.Text;

namespace Hanno.Serialization
{
	public class SafeStringDeserializer : IDeserializer
	{
		private readonly IDeserializer _innerDeserializer;
		private readonly Encoding _encoding;

		public SafeStringDeserializer(IDeserializer innerDeserializer, Encoding encoding)
		{
			if (innerDeserializer == null) throw new ArgumentNullException("innerDeserializer");
			if (encoding == null) throw new ArgumentNullException("encoding");
			_innerDeserializer = innerDeserializer;
			_encoding = encoding;
		}

		public SafeStringDeserializer(IDeserializer innerDeserializer)
			: this(innerDeserializer, Encoding.UTF8)
		{
		}

		public T Deserialize<T>(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (typeof(T) == typeof(string))
			{
				var bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				stream.Position = 0;
				return (T)(object)_encoding.GetString(bytes, 0, bytes.Length);
			}
			return _innerDeserializer.Deserialize<T>(stream);
		}
	}
}