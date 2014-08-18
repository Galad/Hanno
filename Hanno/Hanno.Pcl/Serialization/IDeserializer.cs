using System.IO;

namespace Hanno.Serialization
{
	public interface IDeserializer
	{
		T Deserialize<T>(Stream stream);
	}
}