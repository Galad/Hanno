using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hanno.IO;
using Hanno.Serialization;

namespace Hanno.Storage
{
	public class AsyncFromStringKeyValuePairFilesDataTable<TKey, TValue> : IAsyncDataTable<TKey, TValue>, IDisposable
	{
		private readonly ISerializer _serializer;
		private readonly IDeserializer _deserializer;
		private readonly IAsyncFileOperations _fileOperations;
		private readonly IAsyncDirectoryOperations _directoryOperations;
		private readonly string _databasePath;
		private readonly string _fullName;
		private readonly Dictionary<string, SemaphoreSlim> _lockers = new Dictionary<string, SemaphoreSlim>();

		public AsyncFromStringKeyValuePairFilesDataTable(
			ISerializer serializer,
			IDeserializer deserializer,
			IAsyncFileOperations fileOperations,
			IAsyncDirectoryOperations directoryOperations,
			string databasePath)
		{
			if (serializer == null) throw new ArgumentNullException("serializer");
			if (deserializer == null) throw new ArgumentNullException("deserializer");
			if (fileOperations == null) throw new ArgumentNullException("fileOperations");
			if (directoryOperations == null) throw new ArgumentNullException("directoryOperations");
			if (string.IsNullOrEmpty(databasePath)) throw new ArgumentNullException("databasePath");
			_serializer = serializer;
			_deserializer = deserializer;
			_fileOperations = fileOperations;
			_directoryOperations = directoryOperations;
			_databasePath = databasePath;
			_fullName = typeof(TValue).FullName;
		}

		public async Task<TValue> Get(TKey key)
		{
			var keyString = GetKeyString(key);
			var path = GetEntryPath(keyString);
			var locker = GetLocker(keyString);
			await locker.WaitAsync();
			try
			{
				using (var stream = await _fileOperations.OpenRead(path))
				{
					var value = _deserializer.Deserialize<TValue>(stream);
					return value;
				}
			}
			catch (FileNotFoundException)
			{
				return default(TValue);
			}
			finally
			{
				locker.Release();
			}
		}

		public async Task AddOrUpdate(TKey key, TValue value)
		{
			await EnsureTableExists();
			var keyString = GetKeyString(key);
			var path = GetEntryPath(keyString);
			var locker = GetLocker(keyString);
			await locker.WaitAsync();
			try
			{
				using (var stream = await _fileOperations.Open(path, FileMode.Create, FileAccess.Write))
				{
					_serializer.Serialize(value, stream);
				}
			 }
			finally
			{
				locker.Release();
			}
		}

		private async Task EnsureTableExists()
		{
			var path = Path.Combine(_databasePath, _fullName);
			if (!(await _directoryOperations.Exists(path)))
			{
				await _directoryOperations.CreateDirectory(path);
			}
		}

		public async Task Delete(TKey key)
		{
			var keyString = GetKeyString(key);
			var path = GetEntryPath(keyString);
			var locker = GetLocker(keyString);
			await locker.WaitAsync();
			try
			{
				await _fileOperations.Delete(path);
			}
			finally
			{
				locker.Release();
			}
		}

		public async Task<KeyValuePair<TKey, TValue>[]> Query(Func<IQueryable<KeyValuePair<TKey, TValue>>, IQueryable<KeyValuePair<TKey, TValue>>> query)
		{
			var path = Path.Combine(_databasePath, _fullName);
			if (!(await _directoryOperations.Exists(path)))
			{
				return new KeyValuePair<TKey, TValue>[] { };
			}

			var files = await _directoryOperations.EnumerateFiles(path);
			var tasks = files.Select(async file =>
			{
				var keyString = Path.GetFileName(file);
				var bytes = Encoding.UTF8.GetBytes(keyString);
				TKey key = default(TKey);
				using (var stream = new MemoryStream(bytes))
				{
					key = _deserializer.Deserialize<TKey>(stream);
				}
				TValue value = default(TValue);
				var locker = GetLocker(keyString);
				await locker.WaitAsync();
				try
				{
					using (var stream = await _fileOperations.OpenRead(file))
					{
						value = _deserializer.Deserialize<TValue>(stream);
					}
					var result = new KeyValuePair<TKey, TValue>(key, value);
					return result;
				}
				finally
				{
					locker.Release();
				}
			});
			var tasksResult = await Task.WhenAll(tasks);
			return query(tasksResult.AsQueryable()).ToArray();
		}

		public async Task<bool> Contains(TKey key)
		{

			var keyString = GetKeyString(key);
			var locker = GetLocker(keyString);
			await locker.WaitAsync();
			try
			{
				using (await _fileOperations.OpenRead(GetEntryPath(keyString)))
				{
					return true;
				}
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			finally
			{
				locker.Release();
			}
		}

		public string GetKeyString(TKey key)
		{
			using (var stream = new MemoryStream())
			{
				_serializer.Serialize(key, stream);
				var bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				var s = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				return s;
			}
		}

		public string GetEntryPath(string key)
		{
			return Path.Combine(_databasePath, _fullName, key);
		}

		private readonly object _dictionaryLocker = new object();
		private SemaphoreSlim GetLocker(string keyString)
		{
			lock (_dictionaryLocker)
			{
				SemaphoreSlim locker;
				if (_lockers.TryGetValue(keyString, out locker))
				{
					return locker;

				}
				_lockers[keyString] = locker = new SemaphoreSlim(1);
				return locker;
			}
		}

		#region Dispose
		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't 
		// own unmanaged resources itself, but leave the other methods
		// exactly as they are. 
		~AsyncFromStringKeyValuePairFilesDataTable()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}
		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

			}
			foreach (var readerWriterLockSlim in _lockers.Values)
			{
				readerWriterLockSlim.Dispose();
			}
			_lockers.Clear();
		}
		#endregion

	}
}