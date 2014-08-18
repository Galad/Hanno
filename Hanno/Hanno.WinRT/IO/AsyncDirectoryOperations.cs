//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Windows.Storage;

//namespace Hanno.IO
//{
//	public class AsyncDirectoryOperations : IAsyncDirectoryOperations
//	{
//		private readonly IStorageFolder _startingFolder;

//		public AsyncDirectoryOperations(IStorageFolder startingFolder)
//		{
//			if (startingFolder == null) throw new ArgumentNullException("startingFolder");
//			_startingFolder = startingFolder;
//		}

//		public async Task<bool> Exists(string directory)
//		{
//			try
//			{
//				await _startingFolder.GetFolderAsync(directory);
//				return true;
//			}
//			catch (Exception)
//			{
//				return false;
//			}
//		}

//		public async Task<string[]> EnumerateFiles(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null)
//		{
//			if (selector == null)
//			{
//				selector = _ => _;
//			}
//			var folder = await _startingFolder.GetFolderAsync(directory);
//			var files = await folder.GetFilesAsync();
//			return selector(files.Select(f => f.Path)).ToArray();
//		}

//		public async Task<string[]> EnumerateFiles(string directory, string searchPattern, Func<IEnumerable<string>, IEnumerable<string>> selector = null, bool allDirectories = false)
//		{
//			return await EnumerateFiles(directory, selector);
//		}

//		public async Task CreateDirectory(string directory)
//		{
//			await _startingFolder.CreateFolderAsync(directory, CreationCollisionOption.OpenIfExists);
//		}

//		public async Task<string[]> EnumerateDirectories(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null)
//		{
//			if (selector == null)
//			{
//				selector = _ => _;
//			}
//			var folder = await _startingFolder.GetFolderAsync(directory);
//			var folders = await folder.GetFoldersAsync();
//			return selector(folders.Select(f => f.Path)).ToArray();
//		}

//		public async Task DeleteDirectory(string directory)
//		{
//			await _startingFolder.DeleteAsync();
//		}
//	}
//}