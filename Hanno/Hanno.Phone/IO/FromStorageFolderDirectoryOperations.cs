using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
#if !NETFX_CORE
using PathConstants = System.IO.Path;
#endif

namespace Hanno.IO
{
#if NETFX_CORE
	public class PathConstants
	{
		public const char DirectorySeparatorChar = '\\';
	}
#endif
	public sealed class FromStorageFolderDirectoryOperations : IAsyncDirectoryOperations
	{
		private readonly StorageFolder _storageFolder;

		public FromStorageFolderDirectoryOperations(StorageFolder storageFolder)
		{
			if (storageFolder == null) throw new ArgumentNullException("storageFolder");
			_storageFolder = storageFolder;
		}

		public async Task<bool> Exists(string directory)
		{
			try
			{
				await _storageFolder.GetFolderAsync(directory);
				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

		public async Task<string[]> EnumerateFiles(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null)
		{
			var folder = await _storageFolder.GetFolderAsync(directory);
			var files = await folder.GetFilesAsync();
			var fileNames = files.Select(file => Path.Combine(directory, file.Name)).ToArray();
			return fileNames;
		}

		public async Task<string[]> EnumerateFiles(string directory, string searchPattern, Func<IEnumerable<string>, IEnumerable<string>> selector = null, bool allDirectories = false)
		{
			if (allDirectories)
			{
				throw new NotSupportedException();
			}
			var folder = await _storageFolder.GetFolderAsync(directory);
			var files = await folder.GetFilesAsync();
			var fileNames = selector(files.Select(file => file.Name)).ToArray();
			return fileNames;
		}

		public async Task CreateDirectory(string directory)
		{
			var subFolderParts = directory.Split(PathConstants.DirectorySeparatorChar).ToArray();
			var subFolder = string.Join(PathConstants.DirectorySeparatorChar.ToString(), subFolderParts.Take(subFolderParts.Length - 1).Skip(1));
			StorageFolder folder;
			if (string.IsNullOrEmpty(subFolder))
			{
				folder = _storageFolder;
			}
			else
			{
				folder = await _storageFolder.GetFolderAsync(subFolderParts.Last());
			}
			await folder.CreateFolderAsync(directory, CreationCollisionOption.OpenIfExists);
		}

		public async Task<string[]> EnumerateDirectories(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null)
		{
			var folder = await _storageFolder.GetFolderAsync(directory);
			var folders = await folder.GetFoldersAsync();
			var folderNames = folders.Select(f => f.Name).ToArray();
			return folderNames;
		}

		public async Task DeleteDirectory(string directory)
		{
			try
			{
				var folder = await _storageFolder.GetFolderAsync(directory);
				await folder.DeleteAsync();
			}
#if !NETFX_CORE
			catch (DirectoryNotFoundException)
			{
			} 
#else
			finally
			{
			}
#endif
		}
	}

	public sealed class FromStorageFolderFileOperations : IAsyncFileOperations
	{

		private readonly StorageFolder _storageFolder;

		public FromStorageFolderFileOperations(StorageFolder storageFolder)
		{
			if (storageFolder == null) throw new ArgumentNullException("storageFolder");
			_storageFolder = storageFolder;
		}

		public async Task<Stream> OpenRead(string path)
		{
			var file = await GetStorageFileFromPath(path);
			return await file.OpenStreamForReadAsync();
		}

		public async Task Delete(string path)
		{
			try
			{
				var file = await GetStorageFileFromPath(path);
				await file.DeleteAsync();
			}
			catch (FileNotFoundException)
			{
			}
#if !NETFX_CORE
			catch (DirectoryNotFoundException)
			{
			} 
#endif
		}

		public async Task WriteAllBytes(string path, byte[] bytes)
		{
			var file = await CreateStorageFileFromPath(path, CreationCollisionOption.ReplaceExisting);
			using (var stream = await file.OpenStreamForWriteAsync())
			{
				await stream.WriteAsync(bytes, 0, bytes.Length);
			}
		}

		public async Task<Stream> OpenWrite(string path)
		{
			var file = await CreateStorageFileFromPath(path, CreationCollisionOption.OpenIfExists);
			return await file.OpenStreamForWriteAsync();
		}

		public async Task<Stream> Open(string path, FileMode fileMode, FileAccess fileAccess)
		{
			IStorageFile storageFile;
			switch (fileMode)
			{
				case FileMode.Open:
					storageFile = await GetStorageFileFromPath(path);
					break;
				case FileMode.OpenOrCreate:
					storageFile = await CreateStorageFileFromPath(path, CreationCollisionOption.OpenIfExists);
					break;
				case FileMode.Create:
				case FileMode.Truncate:
					storageFile = await CreateStorageFileFromPath(path, CreationCollisionOption.ReplaceExisting);
					break;
				case FileMode.CreateNew:
					storageFile = await CreateStorageFileFromPath(path, CreationCollisionOption.FailIfExists);
					break;
				default:
					throw new NotSupportedException();
			}
			if (fileAccess == FileAccess.Read)
			{
				return await storageFile.OpenStreamForReadAsync();
			}
			if (fileAccess == FileAccess.Write)
			{
				return await storageFile.OpenStreamForWriteAsync();
			}
			var randomStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
			return randomStream.AsStream();
		}

		private async Task<StorageFile> GetStorageFileFromPath(string path)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			var folder = await GetStorageFolderFromFilePath(path);
			var file = await folder.GetFileAsync(Path.GetFileName(path).Replace('.', '_'));
			return file;
		}

		private async Task<StorageFile> CreateStorageFileFromPath(string path, CreationCollisionOption createCollisionOption)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			var folder = await GetStorageFolderFromFilePath(path);
			// '.' chars results in an NotAuthorizedAccessException
			var file = await folder.CreateFileAsync(Path.GetFileName(path).Replace('.', '_'), createCollisionOption);
			return file;
		}

		private async Task<StorageFolder> GetStorageFolderFromFilePath(string path)
		{
			var folderPath = Path.GetDirectoryName(path);
			StorageFolder folder;
			if (string.IsNullOrEmpty(folderPath))
			{
				folder = _storageFolder;
			}
			else
			{
				folder = await _storageFolder.GetFolderAsync(folderPath);
			}
			return folder;
		}

		public async Task Move(string sourcePath, string targetPath)
		{
			var file = await GetStorageFileFromPath(sourcePath);
			var targetFolder = await GetStorageFolderFromFilePath(targetPath);
			await file.MoveAsync(targetFolder, Path.GetFileName(targetPath));
		}

		public async Task<bool> Exists(string path)
		{
			try
			{
				var file = await GetStorageFileFromPath(path);				
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			return true;
		}
	}
}