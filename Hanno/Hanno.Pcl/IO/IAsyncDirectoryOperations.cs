using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hanno.IO
{
	public interface IAsyncDirectoryOperations
	{
		Task<bool> Exists(string directory);
		Task<string[]> EnumerateFiles(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null);
		Task<string[]> EnumerateFiles(string directory, string searchPattern, Func<IEnumerable<string>, IEnumerable<string>> selector = null, bool allDirectories = false);
		Task CreateDirectory(string directory);
		Task<string[]> EnumerateDirectories(string directory, Func<IEnumerable<string>, IEnumerable<string>> selector = null);
		Task DeleteDirectory(string directory);
	}
}