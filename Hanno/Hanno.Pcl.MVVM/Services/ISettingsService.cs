using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Services
{
	public interface ISettingsService
	{
		IObservable<T> ObserveValue<T>(string key, Func<T> defaultValue);
		Task SetValue<T>(string key, T value, CancellationToken ct);
	}
}