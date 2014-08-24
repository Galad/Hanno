using System;
using System.Collections;
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
using Hanno.Concurrency;

namespace Hanno.ViewModels
{
	public class ObservableViewModel<T> : IObservableViewModel<T>, IDisposable
	{
		public Func<CancellationToken, Task<T>> Source { get; private set; }
		public Func<T, bool> EmptyPredicate { get; private set; }
		public IObservable<Unit> RefreshOn { get; private set; }
		public TimeSpan TimeoutDelay { get; private set; }

		private readonly Subject<ObservableViewModelNotification> _notificationsSubject = new Subject<ObservableViewModelNotification>();
		private readonly IConnectableObservable<ObservableViewModelNotification> _notificationsObservable;
		private readonly IDisposable _notificationDisposable;
		private readonly CompositeDisposable _subscriptions;
		private readonly ISchedulers _schedulers;
		private readonly SerialDisposable _currentExecution;

		public ObservableViewModel(
			Func<CancellationToken, Task<T>> sourceFactory,
			Func<T, bool> emptyPredicate,
			IObservable<Unit> refreshOn,
			TimeSpan timeout,
			CompositeDisposable subscriptions,
			ISchedulers schedulers)
		{
			if (sourceFactory == null) throw new ArgumentNullException("sourceFactory");
			if (emptyPredicate == null) throw new ArgumentNullException("emptyPredicate");
			if (refreshOn == null) throw new ArgumentNullException("refreshOn");
			if (subscriptions == null) throw new ArgumentNullException("subscriptions");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			_subscriptions = subscriptions;
			_schedulers = schedulers;
			Source = sourceFactory;
			EmptyPredicate = emptyPredicate;
			RefreshOn = refreshOn;
			TimeoutDelay = timeout;
			RefreshOn.SelectMany(async _ =>
			{
				await RefreshAsync();
				return Unit.Default;
			})
					 .Subscribe(_ => { }, e => { }, () => { })
					 .DisposeWith(_subscriptions);

			//intialization
			_notificationsObservable = _notificationsSubject.Replay(1, ImmediateScheduler.Instance);
			_notificationDisposable = _notificationsObservable.Connect();
			_notificationsSubject.OnNext(new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Initialized,
				Value = null
			});
			_currentExecution = new SerialDisposable();
		}

		public async Task RefreshAsync()
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var disposables = new CompositeDisposable(new CancellationDisposable(cancellationTokenSource));
			_currentExecution.Disposable = disposables;
			using (disposables)
			{
				_notificationsSubject.OnNext(new ObservableViewModelNotification()
				{
					Status = ObservableViewModelStatus.Updating,
					Value = null
				});
				ObservableViewModelNotification notification;
				bool cancelledFromTimeout = false;
				try
				{
					if (TimeoutDelay > TimeSpan.Zero)
					{
						Observable.Timer(TimeoutDelay, _schedulers.ThreadPool.SchedulerFromPriority(SchedulerPriority.Low))
						          .Subscribe(_ =>
						          {
							          cancelledFromTimeout = true;
							          cancellationTokenSource.Cancel();
						          })
						          .DisposeWith(disposables);
					}
					var value = await Source(cancellationTokenSource.Token);
					cancellationTokenSource.Token.ThrowIfCancellationRequested();
					notification = SelectValue(value);
					CurrentValue = notification.Status == ObservableViewModelStatus.Value ? value : default(T);
				}
				catch (OperationCanceledException)
				{
					if (!cancelledFromTimeout)
					{
						return;
					}
					notification = new ObservableViewModelNotification()
					{
						Status = ObservableViewModelStatus.Timeout,
						Value = null
					};
					CurrentValue = default(T);
				}
				catch (Exception ex)
				{
					notification = new ObservableViewModelNotification()
					{
						Status = ObservableViewModelStatus.Error,
						Value = ex
					};
					CurrentValue = default(T);
				}
				_notificationsSubject.OnNext(notification);
			}
		}

		public async void Refresh()
		{
			try
			{
				await RefreshAsync();
			}
			catch (Exception)
			{
			}
		}

		public void Accept(IObservableViewModelVisitor visitor)
		{
			if (visitor == null) throw new ArgumentNullException("visitor");
			visitor.Visit(this);
		}

		private ObservableViewModelNotification SelectValue(T arg)
		{
			return EmptyPredicate(arg) ?
					   new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Empty,
				Value = null
			} :
					   new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Value,
				Value = arg
			};
		}

		public IDisposable Subscribe(IObserver<ObservableViewModelNotification> observer)
		{
			return _notificationsObservable.Subscribe(observer);
		}

		public void ChainEmptyPredicate(Func<T, bool> chainedPredicate)
		{
			//capture empty predicate in the current context
			var previousPredicate = EmptyPredicate;
			EmptyPredicate = arg =>
			{
				var isEmpty = previousPredicate(arg);
				if (isEmpty)
				{
					return true;
				}
				return chainedPredicate(arg);
			};
		}

		public void Dispose()
		{
			_notificationsSubject.OnCompleted();
			_notificationsSubject.Dispose();
			_notificationDisposable.Dispose();
			_subscriptions.Dispose();
		}

		public T CurrentValue { get; private set; }
	}
}