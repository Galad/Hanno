using System.IO;
using Hanno.Serialization;
using Newtonsoft.Json;

namespace Hanno.Pcl.Json
{
	public class NewtonSoftJsonSerializer : IDeserializer, ISerializer
	{
		private readonly JsonSerializer _jsonDeserializer;

		public NewtonSoftJsonSerializer()
		{
			_jsonDeserializer = JsonSerializer.CreateDefault();
		}

		public T Deserialize<T>(Stream stream)
		{
			var reader = new JsonTextReader(new StreamReader(stream));
			var result = _jsonDeserializer.Deserialize<T>(reader);
			stream.Position = 0;
			return result;
		}

		public void Serialize<T>(T value, Stream stream)
		{
			var writer = new JsonTextWriter(new StreamWriter(stream));
			_jsonDeserializer.Serialize(writer, value);
			writer.Flush();
			stream.Position = 0;
		}
	}
}
