using System;
using System.ComponentModel;
using System.IO;

namespace Hanno.IO
{
	public sealed class FileOperations : IFileOperations
	{
		public Stream OpenRead(string path)
		{
			try
			{
				return File.OpenRead(path);
			}
			catch (DirectoryNotFoundException ex)
			{
				throw new FileNotFoundException("Folder does not exists", ex);
			}
		}

		public void Delete(string path)
		{
			File.Delete(path);
		}

		public void WriteAllBytes(string path, byte[] bytes)
		{
			File.WriteAllBytes(path, bytes);
		}

		public Stream OpenWrite(string path)
		{
			return File.OpenWrite(path);
		}

		public Stream Open(string path, FileMode fileMode, FileAccess fileAccess)
		{
			return File.Open(path, GetNativeFileMode(fileMode), GetNativeFileAccess(fileAccess));
		}

		private System.IO.FileAccess GetNativeFileAccess(FileAccess fileAccess)
		{
			switch (fileAccess)
			{
				case FileAccess.Read:
					return System.IO.FileAccess.Read;
				case FileAccess.Write:
					return System.IO.FileAccess.Write;
				case FileAccess.ReadWrite:
					return System.IO.FileAccess.ReadWrite;
				default:
					throw new NotSupportedException(string.Format("File access {0} is not supported", fileAccess));
			}
		}

		public static System.IO.FileMode GetNativeFileMode(FileMode fileMode)
		{
			switch (fileMode)
			{
				case FileMode.Append:
					return System.IO.FileMode.Append;
				case FileMode.Create:
					return System.IO.FileMode.Create;
				case FileMode.CreateNew:
					return System.IO.FileMode.CreateNew;
				case FileMode.Open:
					return System.IO.FileMode.Open;
				case FileMode.OpenOrCreate:
					return System.IO.FileMode.OpenOrCreate;
				case FileMode.Truncate:
					return System.IO.FileMode.Truncate;
				default:
					throw new NotSupportedException(string.Format("File mode {0} is not supported", fileMode));
			}
		}

		public void Move(string sourcePath, string targetPath)
		{
			File.Move(sourcePath, targetPath);
		}

		public bool Exists(string path)
		{
			return File.Exists(path);
		}
	}
}