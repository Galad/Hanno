//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Windows.Storage;

//namespace Hanno.IO
//{
//	public class AsyncFileOperations : IAsyncFileOperations
//	{
//		private readonly IStorageFolder _startingFolder;

//		public AsyncFileOperations(IStorageFolder startingFolder)
//		{
//			if (startingFolder == null) throw new ArgumentNullException("startingFolder");
//			_startingFolder = startingFolder;
//		}

//		public async Task<Stream> OpenRead(string path)
//		{
//			var file = await _startingFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
//			var stream = await file.OpenReadAsync();
//			return stream.AsStreamForRead();
//		}

//		public async Task Delete(string path)
//		{
//			try
//			{
//				var file = await _startingFolder.GetFileAsync(path);
//				await file.DeleteAsync();
//			}
//			catch (Exception)
//			{
//			}
//		}

//		public async Task WriteAllBytes(string path, byte[] bytes)
//		{
//			var file = await _startingFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
//			using (var stream = await file.OpenStreamForWriteAsync())
//			{
//				await stream.WriteAsync(bytes, 0, bytes.Length);
//			}
//		}

//		public async Task<Stream> OpenWrite(string path)
//		{
//			var file = await _startingFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
//			var stream = await file.OpenStreamForWriteAsync();
//			return stream;
//		}

//		public Task<Stream> Open(string path, FileMode fileMode, FileAccess fileAccess)
//		{
//			if(fileMode == FileMode.)
//		}
//	}
//}