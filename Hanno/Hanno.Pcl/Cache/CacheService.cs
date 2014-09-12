using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Cache
{
	/// <summary>
	/// Cache service using a IAsyncStorage as its repository
	/// </summary>
	public sealed class CacheService : ICacheService
	{
		private class RunningTask
		{
			public string Key;
			public IEnumerable<KeyValuePair<string, string>> Attributes;
			public TimeSpan MaxAge;
			public object Task;
		}

		private class RunningSemaphore
		{
			public string Key;
			public IEnumerable<KeyValuePair<string, string>> Attributes;
			public TimeSpan MaxAge;
			public SemaphoreSlim Semaphore;
		}

		private readonly ICacheEntryRepository _cacheEntryRepository;
		private readonly List<RunningTask> _runningTasks = new List<RunningTask>();
		private readonly List<RunningSemaphore> _runningSemaphores = new List<RunningSemaphore>();

		public CacheService(
			ICacheEntryRepository cacheEntryRepository)
		{
			if (cacheEntryRepository == null) throw new ArgumentNullException("cacheEntryRepository");
			_cacheEntryRepository = cacheEntryRepository;
		}

		public async Task<T> ExecuteWithCache<T>(CancellationToken ct, string cacheKey, Func<Task<T>> execute, TimeSpan maxAge, IDictionary<string, string> attributes = null)
		{
			attributes = attributes ?? new Dictionary<string, string>();
			var semaphore = AcquireRunningSemaphore(cacheKey, attributes, maxAge);
			await semaphore.Semaphore.WaitAsync(ct);
			var runningTask = GetRunningTask<T>(cacheKey, attributes, maxAge);
			if (runningTask != null)
			{
				semaphore.Semaphore.Release();
				return await ((Task<T>)runningTask.Task);
			}
			var taskCompletionSource = new TaskCompletionSource<T>();
			runningTask = new RunningTask()
			{
				Key = cacheKey,
				Attributes = attributes,
				MaxAge = maxAge,
				Task = taskCompletionSource.Task
			};
			AddRunningTask(runningTask);
			semaphore.Semaphore.Release();
			try
			{
				var result = await ExecuteWithCacheInternal(cacheKey, execute, maxAge, attributes);
				taskCompletionSource.SetResult(result);
				return result;
			}
			catch (Exception ex)
			{
				taskCompletionSource.SetException(ex);
				throw;
			}
			finally
			{
				RemoveRunningTask(runningTask);
				RemoveSemaphore(semaphore);
			}
		}

		private void AddRunningTask(RunningTask runningTask)
		{
			lock (_runningTasks)
			{
				_runningTasks.Add(runningTask);
			}
		}

		private void RemoveRunningTask(RunningTask runningTask)
		{
			lock (_runningTasks)
			{
				_runningTasks.Remove(runningTask);
			}
		}

		private void RemoveSemaphore(RunningSemaphore runningSemaphore)
		{
			lock (_runningSemaphores)
			{
				_runningSemaphores.Remove(runningSemaphore);
			}
		}

		private async Task<T> ExecuteWithCacheInternal<T>(string cacheKey, Func<Task<T>> execute, TimeSpan maxAge, IDictionary<string, string> attributes)
		{
			var existingEntry = await _cacheEntryRepository.Get<T>(cacheKey, attributes);
			if (existingEntry == null)
			{
				var result = await execute();
				await AddNewCacheEntry(cacheKey, attributes, result);
				return result;
			}
			var now = NowContext.Current.Now;
			if (existingEntry.DateCreated.Add(maxAge) < now)
			{
				var result = await execute();
				await _cacheEntryRepository.Remove(existingEntry.Id);
				await AddNewCacheEntry(cacheKey, attributes, result);
				return result;
			}
			return existingEntry.Value;
		}

		private RunningSemaphore AcquireRunningSemaphore(string cacheKey, IEnumerable<KeyValuePair<string, string>> attributes, TimeSpan maxAge)
		{
			RunningSemaphore semaphore = null;
			var orderedAttributes = attributes.OrderBy(a => a.Key).ToArray();
			lock (_runningSemaphores)
			{
				semaphore = _runningSemaphores.FirstOrDefault(
					r => r.Key == cacheKey &&
						 r.MaxAge == maxAge &&
						 r.Attributes.OrderBy(a => a.Key).SequenceEqual(orderedAttributes)
					);
				if (semaphore == null)
				{
					semaphore = new RunningSemaphore()
					{
						Attributes = orderedAttributes,
						Key = cacheKey,
						MaxAge = maxAge,
						Semaphore = new SemaphoreSlim(1)
					};
					_runningSemaphores.Add(semaphore);
				}
			}
			return semaphore;
		}

		private RunningTask GetRunningTask<T>(string key, IEnumerable<KeyValuePair<string, string>> attributes, TimeSpan maxAge)
		{
			RunningTask runningTask = null;
			var orderedAttributes = attributes.OrderBy(a => a.Key);
			lock (_runningTasks)
			{
				runningTask = _runningTasks.FirstOrDefault(
					r => r.Key == key &&
						 r.MaxAge == maxAge &&
						 r.Attributes.OrderBy(a => a.Key).SequenceEqual(orderedAttributes)
					);
			}
			return runningTask;
		}

		private async Task AddNewCacheEntry<T>(string cacheKey, IDictionary<string, string> attributes, T result)
		{
			var entry = new CacheEntry<T>(Guid.NewGuid(), cacheKey, attributes ?? new Dictionary<string, string>(), NowContext.Current.Now, result);
			await _cacheEntryRepository.AddOrUpdate(entry);
		}

		public async Task Invalidate<T>(CancellationToken ct, string cacheKey, IDictionary<string, string> attributes = null)
		{
			var entry = await _cacheEntryRepository.Get<T>(cacheKey, attributes ?? new Dictionary<string, string>());
			if (entry != null)
			{
				await _cacheEntryRepository.Remove(entry.Id);
			}
		}

		public async Task Invalidate<T>(CancellationToken ct, string cacheKey, TimeSpan minAge, IDictionary<string, string> attributes = null)
		{
			var entry = await _cacheEntryRepository.Get<T>(cacheKey, attributes ?? new Dictionary<string, string>());
			var now = NowContext.Current.Now;
			if (entry != null && entry.DateCreated.Add(minAge) < now)
			{
				await _cacheEntryRepository.Remove(entry.Id);
			}
		}
	}
}