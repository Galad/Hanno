using System.IO;
using System.Threading.Tasks;

namespace Hanno.IO
{
	public interface IAsyncFileOperations
	{
		Task<Stream> OpenRead(string path);
		Task Delete(string path);
		Task WriteAllBytes(string path, byte[] bytes);
		Task<Stream> OpenWrite(string path);
		Task<Stream> Open(string path, FileMode fileMode, FileAccess fileAccess);
	}
}