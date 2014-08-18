namespace Hanno.IO.Compression
{
    public interface ICompressionService
    {
        void Compress(string sourcePath, string destinationFile, bool includeRootDirectory = false, bool overwrite = false);
        void Decompress(string sourcePath, string destinationPath);
    }
}