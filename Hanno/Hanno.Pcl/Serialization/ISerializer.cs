using System.IO;
namespace Hanno.Serialization
{
	public interface ISerializer
	{
		void Serialize<T>(T value, Stream stream);
	}
}