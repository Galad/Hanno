using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Extensions;

namespace Hanno.Serialization
{
	public class FormattableSerializer : ISerializer
	{
		private readonly ISerializer _stringSerializer;
		private readonly ISerializer _innerSerializer;

		public FormattableSerializer(ISerializer stringSerializer, ISerializer innerSerializer)
		{
			if (stringSerializer == null) throw new ArgumentNullException("stringSerializer");
			if (innerSerializer == null) throw new ArgumentNullException("innerSerializer");
			_stringSerializer = stringSerializer;
			_innerSerializer = innerSerializer;
		}

		public void Serialize<T>(T value, Stream stream)
		{
			if (typeof(T).IsAssignableTo<IFormattable>())
			{
				var s = ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);
				_stringSerializer.Serialize(s, stream);
				return;
			}
			_innerSerializer.Serialize(value, stream);
		}
	}

	public abstract class ParsingDeserializer<TValue> : IDeserializer
	{
		private readonly IDeserializer _stringDeserializer;
		private readonly IDeserializer _innerDeserializer;
		private readonly Func<string, TValue> _parsing;

		protected ParsingDeserializer(
			IDeserializer stringDeserializer,
			IDeserializer innerDeserializer,
			Func<string, TValue> parsing)
		{
			if (stringDeserializer == null) throw new ArgumentNullException("stringDeserializer");
			if (innerDeserializer == null) throw new ArgumentNullException("innerDeserializer");
			if (parsing == null) throw new ArgumentNullException("parsing");
			_stringDeserializer = stringDeserializer;
			_innerDeserializer = innerDeserializer;
			_parsing = parsing;
		}

		public T Deserialize<T>(Stream stream)
		{
			if (typeof(T) == typeof(TValue))
			{
				var s = _stringDeserializer.Deserialize<string>(stream);
				return (T)(object)_parsing(s);
			}
			return _innerDeserializer.Deserialize<T>(stream);
		}
	}

	public class ShortDeserializer : ParsingDeserializer<short>
	{
		public ShortDeserializer(IDeserializer stringDeserializer, IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, s => short.Parse(s, CultureInfo.InvariantCulture))
		{
		}
	}
	public class IntDeserializer : ParsingDeserializer<int>
	{
		public IntDeserializer(IDeserializer stringDeserializer, IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, s => int.Parse(s, CultureInfo.InvariantCulture))
		{
		}
	}
	public class LongDeserializer : ParsingDeserializer<long>
	{
		public LongDeserializer(IDeserializer stringDeserializer, IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, s => long.Parse(s, CultureInfo.InvariantCulture))
		{
		}
	}
	public class BooleanDeserializer : ParsingDeserializer<bool>
	{
		public BooleanDeserializer(IDeserializer stringDeserializer, IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, s => bool.Parse(s))
		{
		}
	}
	public class FloatDeserializer : ParsingDeserializer<float>
	{
		public FloatDeserializer(IDeserializer stringDeserializer, IDeserializer innerDeserializer)
			: base(stringDeserializer, innerDeserializer, s => float.Parse(s, CultureInfo.InvariantCulture))
		{
		}
	}

	public class ValueTypeDeserializer : IDeserializer
	{
		private readonly IDeserializer _deserializer;

		public ValueTypeDeserializer(IDeserializer defaultDeserializer, IDeserializer stringDeserializer)
		{
			if (defaultDeserializer == null) throw new ArgumentNullException("defaultDeserializer");
			if (stringDeserializer == null) throw new ArgumentNullException("stringDeserializer");
			_deserializer =
				new BooleanDeserializer(stringDeserializer,
					new IntDeserializer(stringDeserializer,
						new ShortDeserializer(stringDeserializer,
							new LongDeserializer(stringDeserializer,
								new FloatDeserializer(stringDeserializer,
									defaultDeserializer)))));
		}

		public T Deserialize<T>(Stream stream)
		{
			return _deserializer.Deserialize<T>(stream);
		}
	}

	public class ValueTypeSerializer : ISerializer
	{
		private readonly ISerializer _serializer;

		public ValueTypeSerializer(ISerializer defaultSerializer, ISerializer stringSerializer)
		{
			if (defaultSerializer == null) throw new ArgumentNullException("defaultSerializer");
			if (stringSerializer == null) throw new ArgumentNullException("stringSerializer");
			_serializer =
				new FormattableSerializer(stringSerializer,
					defaultSerializer);
		}

		public void Serialize<T>(T value, Stream stream)
		{
			_serializer.Serialize(value, stream);
		}
	}
}
