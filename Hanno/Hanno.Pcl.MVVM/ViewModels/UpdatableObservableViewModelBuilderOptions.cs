using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class UpdatableObservableViewModelBuilderOptions<T, TCollection> :
		IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> where TCollection : IEnumerable<T>
	{
		private readonly IScheduler _actionSelectionScheduler;
		private readonly Action<IObservableViewModel> _saveViewModel;
		private readonly IScheduler _dispatcherScheduler;
		public Func<CancellationToken, Task<TCollection>> Source { get; private set; }

		public UpdatableObservableViewModelBuilderOptions(Action<IObservableViewModel> saveViewModel, Func<CancellationToken, Task<TCollection>> source, IScheduler actionSelectionScheduler, IScheduler dispatcherScheduler)
		{
			if (saveViewModel == null) throw new ArgumentNullException("saveViewModel");
			if (source == null) throw new ArgumentNullException("source");
			if (actionSelectionScheduler == null) throw new ArgumentNullException("actionSelectionScheduler");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			_saveViewModel = saveViewModel;
			_dispatcherScheduler = dispatcherScheduler;
			_actionSelectionScheduler = actionSelectionScheduler;
			Source = source;
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification> UpdateOn<TNotification>(Func<IObservable<TNotification>> notifications)
		{
			return new UpdatableObservableViewModelBuilderOptions<T, TCollection, TNotification>(
				_saveViewModel,
				Source,
				notifications,
				_actionSelectionScheduler,
				_dispatcherScheduler
				);
		}
	}

	public class UpdatableObservableViewModelBuilderOptions<T, TCollection, TNotification> :
		IUpdatableObservableViewModelBuilderOptions<T>,
		IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification>
		 where TCollection : IEnumerable<T>
	{
		private readonly Action<IObservableViewModel> _saveViewModel;
		private readonly IScheduler _actionSelectionScheduler;
		private readonly IScheduler _dispatcherScheduler;
		public Func<CancellationToken, Task<TCollection>> Source { get; private set; }
		public Func<IObservable<TNotification>> Notifications { get; private set; }
		public Func<TNotification, ObservableCollection<T>, Action> ActionSelector { get; private set; }
		private Func<ObservableCollection<T>, bool> _emptyPredicate = arg => false;
		private IObservable<Unit> _refreshOn = Observable.Empty<Unit>();
		private TimeSpan _timeout = TimeSpan.Zero;
		private bool _refreshOnCollectionUpdateNotification;

		public UpdatableObservableViewModelBuilderOptions(
			Action<IObservableViewModel> saveViewModel,
			Func<CancellationToken, Task<TCollection>> source,
			Func<IObservable<TNotification>> notifications,
			IScheduler actionSelectionScheduler,
			IScheduler dispatcherScheduler)
		{
			if (saveViewModel == null) throw new ArgumentNullException("saveViewModel");
			if (source == null) throw new ArgumentNullException("source");
			if (notifications == null) throw new ArgumentNullException("notifications");
			if (actionSelectionScheduler == null) throw new ArgumentNullException("actionSelectionScheduler");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			Source = source;
			Notifications = notifications;
			_saveViewModel = saveViewModel;
			_actionSelectionScheduler = actionSelectionScheduler;
			_dispatcherScheduler = dispatcherScheduler;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> EmptyPredicate(Func<ObservableCollection<T>, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			_emptyPredicate = predicate;
			return this;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> RefreshOn<TRefresh>(IObservable<TRefresh> refreshTrigger)
		{
			if (refreshTrigger == null) throw new ArgumentNullException("refreshTrigger");
			_refreshOn = refreshTrigger.SelectUnit();
			return this;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> Timeout(TimeSpan timeout)
		{
			_timeout = timeout;
			return this;
		}

		public IUpdatableObservableViewModelBuilderOptions<T> UpdateAction(Func<TNotification, ObservableCollection<T>, Action> updateActionSelector)
		{
			if (updateActionSelector == null) throw new ArgumentNullException("updateActionSelector");
			ActionSelector = updateActionSelector;
			return this;
		}

		public IObservableViewModel<ObservableCollection<T>> ToViewModel()
		{
			var subscription = new CompositeDisposable();
			var updateSubscription = new SerialDisposable().DisposeWith(subscription);
			Func<CancellationToken, Task<ObservableCollection<T>>> source = ct => GetSource(ct, Source, updateSubscription);
			Subject<Unit> subject = _refreshOnCollectionUpdateNotification ? new Subject<Unit>() : null;

			var viewModel = new ObservableViewModel<ObservableCollection<T>>(
				source,
				_emptyPredicate,
				subject ?? _refreshOn,
				_timeout,
				subscription);
			if (_refreshOnCollectionUpdateNotification)
			{
				SubscribeToRefreshOnCollectionUpdateNotification(subscription, viewModel, subject);
			}

			_saveViewModel(viewModel);
			return viewModel;
		}

		private void SubscribeToRefreshOnCollectionUpdateNotification(CompositeDisposable subscription, ObservableViewModel<ObservableCollection<T>> viewModel, IObserver<Unit> observer)
		{
			this.Notifications()
				.SelectMany(n => viewModel.Take(1))
				.Where(n => n.Status == ObservableViewModelStatus.Empty || n.Status == ObservableViewModelStatus.Error || n.Status == ObservableViewModelStatus.Initialized)
				.SelectUnit()
				.Merge(_refreshOn ?? Observable.Empty<Unit>())
				.Do(_ => { }, e => { }, () => { })
				.Subscribe(observer)
				.DisposeWith(subscription);
		}

		private async Task<ObservableCollection<T>> GetSource(CancellationToken ct, Func<CancellationToken, Task<TCollection>> sourceFactory, SerialDisposable disposable)
		{
			ObservableCollection<T> observableCollection;
			var sourceCollection = await sourceFactory(ct);
			if (sourceCollection == null)
			{
				return null;
			}
			disposable.Disposable = sourceCollection
				.AsObservableCollectionWithNotifications(
					Notifications(),
					ActionSelector,
					_actionSelectionScheduler,
					_dispatcherScheduler,
					out observableCollection)
				.Subscribe(_ => { }, e => { }, () => { });
			return observableCollection;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> RefreshOnCollectionUpdateNotification()
		{
			_refreshOnCollectionUpdateNotification = true;
			return this;
		}
	}
}