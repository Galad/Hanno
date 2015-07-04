using System;
using System.IO;

namespace Hanno.Serialization
{
	public class XmlSerializer : ISerializer, IDeserializer
	{
		private Func<Type, System.Xml.Serialization.XmlSerializer> getSerializer;

		public XmlSerializer()
		{
			getSerializer = type => new System.Xml.Serialization.XmlSerializer(type);
		}

		public XmlSerializer(string defaultNamespace)
		{
			getSerializer = type => new System.Xml.Serialization.XmlSerializer(type, defaultNamespace);
		}

		public XmlSerializer(Type[] extraTypes)
		{
			getSerializer = type => new System.Xml.Serialization.XmlSerializer(type, extraTypes);
		}

		public void Serialize<T>(T value, Stream stream)
		{
			var serializer = GetSerializer<T>();
			serializer.Serialize(stream, value);
			stream.Position = 0;
		}

		private System.Xml.Serialization.XmlSerializer GetSerializer<T>()
		{
			return getSerializer(typeof(T));
		}

		public T Deserialize<T>(Stream stream)
		{
			var serializer = GetSerializer<T>();
			var result = (T)serializer.Deserialize(stream);
			return result;
		}
	}
}