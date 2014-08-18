using System.IO;
using System.Threading.Tasks;

namespace Hanno.IO
{
	public interface IAcyncFileOperations
	{
		Task<Stream> OpenRead(string path);
		Task<Stream> OpenWrite(string path);
		Task Delete(string path);
		Task WriteAllBytes(string path, byte[] bytes);
	}
	
}