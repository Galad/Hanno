using System;
using System.Collections.Generic;
using System.IO;
using Hanno.IO;

namespace Hanno.IO
{
	public sealed class DirectoryOperations : IDirectoryOperations
	{
		public bool Exists(string directory)
		{
			return Directory.Exists(directory);
		}

		public IEnumerable<string> EnumerateFiles(string directory)
		{
			return Directory.EnumerateFiles(directory);
		}

		public IEnumerable<string> EnumerateFiles(string directory, string searchPattern, bool allDirectories)
		{
			return Directory.EnumerateFiles(directory, searchPattern, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public void CreateDirectory(string directory)
		{
			if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException("directory");
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		public IEnumerable<string> EnumerateDirectories(string directory)
		{
			return Directory.EnumerateDirectories(directory);
		}

		public void DeleteDirectory(string directory)
		{
			Directory.Delete(directory, true);
		}
	}
}