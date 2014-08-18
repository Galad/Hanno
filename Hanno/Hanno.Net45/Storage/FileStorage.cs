using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Hanno.Serialization;

namespace Hanno.Storage
{

	//public class FileStorage : IStorage
	//{
	//	private ISerializer _serializer;
	//	private IDeserializer _deserializer;
	//	private string _folder;
	//	private readonly IDictionary<string, SemaphoreSlim> semaphores = new Dictionary<string, SemaphoreSlim>();

	//	public FileStorage(string folder, ISerializer serializer, IDeserializer deserializer)
	//	{
	//		fileStorage(folder, serializer, deserializer);
	//	}

	//	public FileStorage(string folder)
	//	{
	//		var serializer = new XmlSerializer();
	//		fileStorage(folder, serializer, serializer);
	//	}

	//	private void fileStorage(string folder, ISerializer serializer, IDeserializer deserializer)
	//	{
	//		if (serializer == null) throw new ArgumentNullException("serializer");
	//		if (deserializer == null) throw new ArgumentNullException("deserializer");
	//		if (!Directory.Exists(folder))
	//		{
	//			Directory.CreateDirectory(folder);
	//		}
	//		if (string.IsNullOrEmpty(folder))
	//		{
	//			throw new ArgumentNullException("folder");
	//		}
	//		_folder = folder;
	//		_serializer = serializer;
	//		_deserializer = deserializer;
	//	}

	//	public async Task Backup<T>(string key, T value)
	//	{
	//		try
	//		{
	//			using (var stream = await GetStream(key, FileMode.Create, FileAccess.Write))
	//			{
	//				await Task.Run(() => _serializer.Serialize(value, stream));
	//			}
	//		}
	//		finally
	//		{
	//			GetSemaphore(key).Release();
	//		}
	//	}

	//	private async Task<FileStream> GetStream(string key, FileMode fileMode, FileAccess fileAccess)
	//	{
	//		await GetSemaphore(key).WaitAsync();
	//		return await Task.Run(() => File.Open(Path.Combine(_folder, key), fileMode, fileAccess));
	//	}

	//	private SemaphoreSlim GetSemaphore(string key)
	//	{
	//		if (!semaphores.ContainsKey(key))
	//		{
	//			semaphores.Add(key, new SemaphoreSlim(1));
	//		}
	//		var semaphore = semaphores[key];
	//		return semaphore;
	//	}

	//	public async Task<T> Restore<T>(string key)
	//	{
	//		try
	//		{
	//			using (var stream = await GetStream(key, FileMode.OpenOrCreate, FileAccess.Read))
	//			{
	//				var result = _deserializer.Deserialize<T>(stream);
	//				return result;
	//			}
	//		}
	//		finally
	//		{
	//			GetSemaphore(key).Release();
	//		}
	//	}

	//	public Task Remove(string key)
	//	{
	//		return Task.Run(() => File.Delete(GetPath(key)));
	//	}

	//	private string GetPath(string key)
	//	{
	//		return Path.Combine(_folder, key);
	//	}

	//	public Task<bool> Contains(string key)
	//	{
	//		return Task.Run(() => File.Exists(GetPath(key)));
	//	}

	//	public Task RemoveGroup(string suffix)
	//	{
	//		return Task.Run(() => { });
	//	}
	//}
}