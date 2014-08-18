using System.IO;

namespace Hanno.IO
{
    public interface IFileOperations
    {
        Stream OpenRead(string path);
        void Delete(string path);
        void WriteAllBytes(string path, byte[] bytes);
        Stream OpenWrite(string path);
	    Stream Open(string path, FileMode fileMode, FileAccess fileAccess);
    }
}