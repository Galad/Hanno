using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class ObservableViewModelBuilder : IObservableViewModelBuilder
	{
		private readonly Action<IObservableViewModel> _saveViewModel;
		private readonly IScheduler _backgroundScheduler;
		private readonly IScheduler _dispatcherScheduler;
		private readonly ISchedulers _schedulers;

		public ObservableViewModelBuilder(Action<IObservableViewModel> saveViewModel, IScheduler backgroundScheduler, IScheduler dispatcherScheduler, ISchedulers schedulers)
		{
			if (saveViewModel == null) throw new ArgumentNullException("saveViewModel");
			if (backgroundScheduler == null) throw new ArgumentNullException("backgroundScheduler");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			_saveViewModel = saveViewModel;
			_backgroundScheduler = backgroundScheduler;
			_dispatcherScheduler = dispatcherScheduler;
			_schedulers = schedulers;
		}

		public IObservableViewModelBuilderOptions<T> Execute<T>(Func<CancellationToken, Task<T>> source)
		{
			if (source == null) throw new ArgumentNullException("source");
			return new ObservableViewModelBuilderOptions<T>(_saveViewModel, source, _schedulers);
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source) where TCollection : IEnumerable<T>
		{
			return new UpdatableObservableViewModelBuilderOptions<T, TCollection>(_saveViewModel, source, _backgroundScheduler, _dispatcherScheduler, _schedulers);
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source, T witness) where TCollection : IEnumerable<T>
		{
			return new UpdatableObservableViewModelBuilderOptions<T, TCollection>(_saveViewModel, source, _backgroundScheduler, _dispatcherScheduler, _schedulers);
		}
	}
}