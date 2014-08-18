using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace System.Linq
{
	public static class EnumerableExtensions
	{
		public static IObservable<Unit> AsObservableCollectionWithNotifications<TItem, TNotification>(
			this IEnumerable<TItem> source,
			IObservable<TNotification> notificationSource,
			Func<TNotification, ObservableCollection<TItem>, Action> actionSelector,
			IScheduler actionSelectorScheduler,
			IScheduler actionExecutionScheduler,
			out ObservableCollection<TItem> observableCollection)
		{
			var collection = new ObservableCollection<TItem>(source);
			//we don't want to access the colletion concurrently to we use the SemaphoreSlim to synchronize the access
			//var sync = new SemaphoreSlim(1).DisposeWith(disposables);
			var observable =
				Observable.Using(
					() => new SemaphoreSlim(1),
					sync => notificationSource.ObserveOn(actionSelectorScheduler)
					                          .SelectMany(async (TNotification notification, CancellationToken ct) =>
					                          {
						                          await sync.WaitAsync(ct);
						                          return actionSelector(notification, collection);
					                          })
					                          .ObserveOn(actionExecutionScheduler)
					                          .Do(action =>
					                          {
						                          if (action == null)
						                          {
							                          action = () => { };
						                          }
						                          action();
						                          sync.Release();
					                          }))
				          .SelectUnit();
			observableCollection = collection;

			return observable;
		}
	}
}