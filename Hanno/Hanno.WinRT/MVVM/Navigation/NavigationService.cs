using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Hanno.Navigation;
using Hanno.ViewModels;

namespace Hanno.MVVM.Navigation
{
	public sealed class NavigationService : INavigationService, IRequestNavigation
	{
		private readonly Frame _frame;
		private readonly IPageDefinitionRegistry _pageDefinitions;
		private readonly IViewModelFactory _viewModelFactory;
		private readonly IScheduler _dispatcherScheduler;
		private readonly Subject<INavigationRequest> _navigatingSubject;
		private readonly Subject<INavigationRequest> _navigatedSubject;
		private readonly NavigationHistory _history;
		private readonly IScheduler _backgroundScheduler;
		private readonly SemaphoreSlim _semaphore;

		public NavigationService(
			Frame frame,
			IPageDefinitionRegistry pageDefinitions,
			IViewModelFactory viewModelFactory,
			IScheduler dispatcherScheduler,
			IScheduler backgroundScheduler)
		{
			if (frame == null) throw new ArgumentNullException("frame");
			if (pageDefinitions == null) throw new ArgumentNullException("pageDefinitions");
			if (viewModelFactory == null) throw new ArgumentNullException("viewModelFactory");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			if (backgroundScheduler == null) throw new ArgumentNullException("backgroundScheduler");
			_frame = frame;
			_pageDefinitions = pageDefinitions;
			_viewModelFactory = viewModelFactory;
			_dispatcherScheduler = dispatcherScheduler;
			_backgroundScheduler = backgroundScheduler;
			_navigatingSubject = new Subject<INavigationRequest>();
			_navigatedSubject = new Subject<INavigationRequest>();
			_semaphore = new SemaphoreSlim(1);
			History = _history = new NavigationHistory(RemoveHistoryEntry, ClearHistory);

			_frame.Navigated += _frame_Navigated;
			//_frame.BackKeyPress += _frame_BackKeyPress;
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
					lastEntry = _history.AliveEntries.LastOrDefault();
					var page = e.Content as FrameworkElement;
					if (lastEntry != null && page != null)
					{
						await _dispatcherScheduler.Run(() => page.DataContext = lastEntry.ViewModel);
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
				await NavigateBackWithAliveRequest(ct);
			}
		}

		private async Task NavigateBackWithAliveRequest(CancellationToken ct)
		{
			var wentBackTask = Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => _frame.Navigated += h, h => _frame.Navigated -= h)
												  .SubscribeOn(_dispatcherScheduler)
												  .Where(args => args.EventArgs.NavigationMode == NavigationMode.Back)
												  .Take(1)
												  .ToTask(CancellationToken.None);
			await _dispatcherScheduler.Run(() => _frame.GoBack());
			await wentBackTask;
		}

		private async Task NavigateBackWithNotAliveRequest(CancellationToken ct, INavigationHistoryEntry previousEntry)
		{
			//entries has no navigation page created so we navigate to the page and remove 
			var last = _history.Entries.Last();
			await Navigate(ct, previousEntry.Request);
			await _dispatcherScheduler.Run(() => _frame.BackStack.RemoveAt(_frame.BackStack.Count - 2));
			//removes the previous entry which was not alive
			await _history.Remove(ct, previousEntry);
			//removes the last entry which was the page where the back navigation was made
			await _history.Remove(ct, last);
		}

		public bool CanNavigateBack { get { return _history.Entries.Count > 1; } }
		public INavigationHistory History { get; private set; }
		public IObservable<INavigationRequest> Navigating { get { return _navigatingSubject; } }
		public IObservable<INavigationRequest> Navigated { get { return _navigatedSubject; } }


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

		public async Task Navigate(CancellationToken ct, INavigationRequest request)
		{
			ct = CancellationToken.None;
			await Task.Run(async () =>
			{
				var isReleased = false;
				await _semaphore.WaitAsync(ct);
				try
				{
					var pageDefinition = _pageDefinitions.GetPageDefinition(request.PageName);
					var viewModel = _viewModelFactory.ResolveViewModel(pageDefinition.ViewModelType);
					viewModel.Initialize(request);

					//create tasks
					var setDataContext = SetPageDataContext(pageDefinition, viewModel).ToTask(ct);
					var navigatingTask = NotifyNavigating(ct, pageDefinition, request);
					var navigationIsStarted = await _dispatcherScheduler.Run(() => _frame.Navigate(pageDefinition.PageType));
					if (!navigationIsStarted)
					{
						throw new InvalidOperationException("Navigation failed");
					}
					
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

					_navigatedSubject.OnNext(request);
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

		private void RegisterViewModelUnload(FrameworkElement pageElement, IViewModel viewModel)
		{
			pageElement.ObserveUnloaded(_dispatcherScheduler)
					   .ObserveOn(_backgroundScheduler)
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
			pageElement.ObserveLoaded(_dispatcherScheduler)
					   .ObserveOn(_backgroundScheduler)
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
							 .Where(args => args.EventArgs.SourcePageType == pageDefinition.PageType && args.EventArgs.NavigationMode == NavigationMode.New)
							 .Do(_ => _navigatingSubject.OnNext(request))
							 .Take(1)
							 .SelectUnit()
							 .ToTask(ct);
		}

		private IObservable<FrameworkElement> SetPageDataContext(PageDefinition pageDefinition, IViewModel viewModel)
		{
			return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => _frame.Navigated += h, h => _frame.Navigated -= h, _dispatcherScheduler)
							 .Where(args => args.EventArgs.SourcePageType == pageDefinition.PageType)
							 .Select(args => ((FrameworkElement)args.EventArgs.Content))
							 .Do(page => page.DataContext = viewModel)
							 .Take(1);
		}
	}
}
