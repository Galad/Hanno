using System.Collections.Generic;

namespace Hanno.IO
{
    public interface IDirectoryOperations
    {
        bool Exists(string directory);
        IEnumerable<string> EnumerateFiles(string directory);
        IEnumerable<string> EnumerateFiles(string directory, string searchPattern, bool allDirectories = false);
        void CreateDirectory(string directory);
        IEnumerable<string> EnumerateDirectories(string directory);
        void DeleteDirectory(string directory);
    }
}