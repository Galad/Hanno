using System;
using System.IO;
using System.Threading.Tasks;

namespace Hanno.IO
{
	public sealed class AsyncOperations : IAsyncFileOperations
	{
		private readonly IFileOperations _synchronousFileOperations;

		public AsyncOperations(IFileOperations synchronousFileOperations)
		{
			if (synchronousFileOperations == null) throw new ArgumentNullException("synchronousFileOperations");
			_synchronousFileOperations = synchronousFileOperations;
		}

		public Task<Stream> OpenRead(string path)
		{
			return Task.Run(() => _synchronousFileOperations.OpenRead(path));
		}

		public Task Delete(string path)
		{
			return Task.Run(() => _synchronousFileOperations.Delete(path));
		}

		public Task WriteAllBytes(string path, byte[] bytes)
		{
			return Task.Run(() => _synchronousFileOperations.WriteAllBytes(path, bytes));
		}

		public Task<Stream> OpenWrite(string path)
		{
			return Task.Run(() => _synchronousFileOperations.OpenWrite(path));
		}

		public Task<Stream> Open(string path, FileMode fileMode, FileAccess fileAccess)
		{
			return Task.Run(() => _synchronousFileOperations.Open(path, fileMode, fileAccess));
		}
	}
}