using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hanno.IO
{
	public sealed class AsyncDirectoryOperations : IAsyncDirectoryOperations
	{
		private readonly IDirectoryOperations _synchronousOperations;

		public AsyncDirectoryOperations(IDirectoryOperations synchronousOperations)
		{
			if (synchronousOperations == null) throw new ArgumentNullException("synchronousOperations");
			_synchronousOperations = synchronousOperations;
		}

		public Task<bool> Exists(string directory)
		{
			return Task.Run(() => _synchronousOperations.Exists(directory));
		}

		public Task<string[]> EnumerateFiles(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector)
		{
			if (selector == null)
			{
				selector = files => files;
			}
			return Task.Run(() => selector(_synchronousOperations.EnumerateFiles(directory)).ToArray());
		}

		public Task<string[]> EnumerateFiles(string directory, string searchPattern, Func<IEnumerable<string>, IEnumerable<string>> selector, bool allDirectories = false)
		{
			if (selector == null)
			{
				selector = files => files;
			}
			return Task.Run(() => selector(_synchronousOperations.EnumerateFiles(directory, searchPattern, allDirectories)).ToArray());
		}

		public Task CreateDirectory(string directory)
		{
			return Task.Run(() => _synchronousOperations.CreateDirectory(directory));
		}

		public Task<string[]> EnumerateDirectories(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null)
		{
			if (selector == null)
			{
				selector = files => files;
			}
			return Task.Run(() => selector(_synchronousOperations.EnumerateDirectories(directory)).ToArray());
		}

		public Task DeleteDirectory(string directory)
		{
			return Task.Run(() => _synchronousOperations.DeleteDirectory(directory));
		}
	}
}