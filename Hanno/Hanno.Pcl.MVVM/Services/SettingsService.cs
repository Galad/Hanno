using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Storage;

namespace Hanno.Services
{
	public class SettingsService : ISettingsService
	{
		private readonly IAsyncStorage _storage;
		private readonly Subject<Tuple<string, object>> _settingChanged = new Subject<Tuple<string, object>>();

		public SettingsService(
			IScheduler scheduler,
			IAsyncStorage storage
			)
		{
			if (storage == null) throw new ArgumentNullException("storage");
			_storage = storage;
		}

		public IObservable<T> ObserveValue<T>(string key, Func<T> defaultValue)
		{
			return Observable.Merge(
				Observable.FromAsync(async (ct) =>
				{
					if (await _storage.Contains<string, T>(key, ct))
					{
						return await _storage.Get<string, T>(key, ct);
					}
					var value = defaultValue();
					await _storage.AddOrUpdate(key, value, ct);
					return value;
				}),
				_settingChanged.Where(k => k.Item1.Equals(key, StringComparison.CurrentCultureIgnoreCase))
				               .Select(t => (T)t.Item2));
		}

		public async Task SetValue<T>(string key, T value, CancellationToken ct)
		{
			await _storage.AddOrUpdate(key, value, ct);
			_settingChanged.OnNext(new Tuple<string, object>(key, value));
		}
	}
}