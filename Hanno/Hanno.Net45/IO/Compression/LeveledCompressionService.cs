using System.IO;
using System.IO.Compression;

namespace Hanno.IO.Compression
{
    public class LeveledCompressionService : ICompressionService
    {
        private readonly CompressionLevel _compressionLevel;

        public LeveledCompressionService(CompressionLevel compressionLevel)
        {
            _compressionLevel = compressionLevel;
        }

        public void Compress(string sourcePath, string destinationFile, bool includeRootDirectory, bool overwrite)
        {
            if (overwrite && File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }
            ZipFile.CreateFromDirectory(sourcePath, destinationFile, _compressionLevel, includeRootDirectory);
        }

        public void Decompress(string sourcePath, string destinationPath)
        {
            ZipFile.ExtractToDirectory(sourcePath, destinationPath);
        }
    }
}