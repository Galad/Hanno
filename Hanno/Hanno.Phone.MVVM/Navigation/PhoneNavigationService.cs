using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.System.Threading;
using Hanno.ViewModels;
using Microsoft.Phone.Controls;
using ThreadPool = Windows.System.Threading.ThreadPool;

namespace Hanno.Navigation
{
	public class PhoneNavigationService : INavigationService, IRequestNavigation
	{
		private readonly PhoneApplicationFrame _frame;
		private readonly IPageDefinitionRegistry _pageDefinitions;
		private readonly IViewModelFactory _viewModelFactory;
		private readonly IScheduler _dispatcherScheduler;
		private readonly Subject<INavigationRequest> _navigatingSubject;
		private readonly Subject<INavigationRequest> _navigatedSubject;

		private readonly SemaphoreSlim _semaphore;
		private readonly NavigationHistory _history;

		public PhoneNavigationService(
			PhoneApplicationFrame frame,
			IPageDefinitionRegistry pageDefinitions,
			IViewModelFactory viewModelFactory,
			IScheduler dispatcherScheduler)
		{
			if (frame == null) throw new ArgumentNullException("frame");
			if (pageDefinitions == null) throw new ArgumentNullException("pageDefinitions");
			if (viewModelFactory == null) throw new ArgumentNullException("viewModelFactory");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			_frame = frame;
			_pageDefinitions = pageDefinitions;
			_viewModelFactory = viewModelFactory;
			_dispatcherScheduler = dispatcherScheduler;
			_navigatingSubject = new Subject<INavigationRequest>();
			_navigatedSubject = new Subject<INavigationRequest>();
			_semaphore = new SemaphoreSlim(1);
			History = _history = new NavigationHistory(RemoveHistoryEntry, ClearHistory);

			_frame.Navigated += _frame_Navigated;
			_frame.BackKeyPress += _frame_BackKeyPress;
		}

		void _frame_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//intercept back navigation and use our back navigation logic
			if (_history.Entries.Count > 1)
			{
				e.Cancel = true;
				new Action(async () => await NavigateBack(CancellationToken.None))();
			}
		}

		async void _frame_Navigated(object sender, NavigationEventArgs e)
		{
			try
			{
				if (e.NavigationMode == NavigationMode.Back)
				{
					var lastEntry = _history.AliveEntries.LastOrDefault();
					if (lastEntry != null)
					{
						await _history.Remove(CancellationToken.None, lastEntry);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public async Task NavigateBack(CancellationToken ct)
		{
			if (!CanNavigateBack)
			{
				throw new InvalidOperationException("Cannot navigate back");
			}
			var previousEntry = _history.Entries[_history.Entries.Count - 2];
			if (!previousEntry.IsAlive)
			{
				await NavigateBackWithNotAliveRequest(ct, previousEntry);
			}
			else
			{
				await NavigateBackWithAliveRequest();
			}
		}

		private async Task NavigateBackWithAliveRequest()
		{
			var wentBackTask = Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => _frame.Navigated += h, h => _frame.Navigated -= h)
												  .SubscribeOn(_dispatcherScheduler)
												  .Where(args => args.EventArgs.NavigationMode == NavigationMode.Back)
												  .Take(1)
												  .ToTask();
			await _dispatcherScheduler.Run(() => _frame.GoBack());
			await wentBackTask;
		}

		public bool CanNavigateBack { get { return _history.Entries.Count > 1; } }

		public INavigationHistory History { get; private set; }

		public IObservable<INavigationRequest> Navigating { get { return _navigatingSubject; } }

		public IObservable<INavigationRequest> Navigated { get { return _navigatedSubject; } }

		public async Task Navigate(CancellationToken ct, INavigationRequest request)
		{
			await Task.Run(async () =>
			{
				var isReleased = false;
				await _semaphore.WaitAsync();
				try
				{
					var pageDefinition = _pageDefinitions.GetPageDefinition(request.PageName);
					var viewModel = _viewModelFactory.ResolveViewModel(pageDefinition.ViewModelType);

					//create tasks
					var setDataContext = SetPageDataContext(pageDefinition, viewModel).ToTask();
					var navigatingTask = NotifyNavigating(ct, pageDefinition, request);
					var navigationIsStarted = await _dispatcherScheduler.Run(() => _frame.Navigate(pageDefinition.Uri));
					if (!navigationIsStarted)
					{
						throw new InvalidOperationException("Navigation failed");
					}

					viewModel.Initialize(request);
					await navigatingTask;
					var pageElement = await setDataContext;
					//since the navigation completed, we can let another navigation start
					_semaphore.Release();
					isReleased = true;

					//add entry to history
					var historyEntry = new AliveNavigationHistoryEntry(request, viewModel);
					_history.Append(historyEntry);

					//list to loaded / unloaded events
					RegisterViewModelLoad(pageElement, viewModel);
					RegisterViewModelUnload(pageElement, viewModel);
				}
				finally
				{
					if (!isReleased)
					{
						_semaphore.Release();
						isReleased = true;
					}
				}
			});
		}

		private async Task NavigateBackWithNotAliveRequest(CancellationToken ct, INavigationHistoryEntry previousEntry)
		{
			//entries has no navigation page created so we navigate to the page and remove 
			var last = _history.Entries.Last();
			await Navigate(ct, previousEntry.Request);
			await _dispatcherScheduler.Run(() => _frame.RemoveBackEntry());
			//removes the previous entry which was not alive
			await _history.Remove(ct, previousEntry);
			//removes the last entry which was the page where the back navigation was made
			await _history.Remove(ct, last);
		}

		private void RegisterViewModelUnload(FrameworkElement pageElement, IViewModel viewModel)
		{
			pageElement.ObserveUnloaded()
					   .SubscribeOn(_dispatcherScheduler)
					   .ObserveOn(ThreadPoolScheduler.Instance)
					   .SelectMany(async (_, ct) =>
					   {
						   await viewModel.Unload(ct);
						   return Unit.Default;
					   })
					   .Subscribe(_ => { }, e => { })
					   .DisposeWith(viewModel.LongDisposables);
		}

		private void RegisterViewModelLoad(FrameworkElement pageElement, IViewModel viewModel)
		{
			pageElement.ObserveLoaded()
					   .SubscribeOn(_dispatcherScheduler)
					   .ObserveOn(ThreadPoolScheduler.Instance)
					   .SelectMany(async (_, ct) =>
					   {
						   await viewModel.Load(ct);
						   return Unit.Default;
					   })
					   .Subscribe(_ => { }, e => { })
					   .DisposeWith(viewModel.LongDisposables);
		}

		private Task NotifyNavigating(CancellationToken ct, PageDefinition pageDefinition, INavigationRequest request)
		{
			return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => _frame.Navigating += h, h => _frame.Navigating -= h)
							 .SubscribeOn(_dispatcherScheduler)
							 .Where(args => args.EventArgs.Uri == pageDefinition.Uri && args.EventArgs.NavigationMode == NavigationMode.New)
							 .Do(_ => _navigatingSubject.OnNext(request))
							 .Take(1)
							 .SelectUnit()
							 .ToTask(ct);
		}

		private IObservable<FrameworkElement> SetPageDataContext(PageDefinition pageDefinition, IViewModel viewModel)
		{
			var sw = new System.Diagnostics.Stopwatch();
			return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => _frame.Navigated += h, h => _frame.Navigated -= h)
							 .SubscribeOn(_dispatcherScheduler)
							 .Where(args => args.EventArgs.Uri == pageDefinition.Uri)
							 .Select(args => ((FrameworkElement)args.EventArgs.Content))
							 .Do(_ => sw.Start())
							 .Do(page => page.DataContext = viewModel)
							 .Do(_ => System.Diagnostics.Debug.WriteLine(sw.ElapsedMilliseconds))
							 .Take(1);
		}



		private Task ClearHistory(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		private async Task RemoveHistoryEntry(CancellationToken ct, INavigationHistoryEntry arg)
		{
			if (arg.IsAlive)
			{
				var aliveEntry = (AliveNavigationHistoryEntry)arg;
				await ThreadPool.RunAsync(operation => _viewModelFactory.ReleaseViewModel(aliveEntry.ViewModel), WorkItemPriority.Low);
			}
		}
	}
}
