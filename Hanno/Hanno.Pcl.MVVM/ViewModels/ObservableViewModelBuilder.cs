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

		public ObservableViewModelBuilder(Action<IObservableViewModel> saveViewModel, IScheduler backgroundScheduler, IScheduler dispatcherScheduler)
		{
			if (saveViewModel == null) throw new ArgumentNullException("saveViewModel");
			if (backgroundScheduler == null) throw new ArgumentNullException("backgroundScheduler");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			_saveViewModel = saveViewModel;
			_backgroundScheduler = backgroundScheduler;
			_dispatcherScheduler = dispatcherScheduler;
		}

		public IObservableViewModelBuilderOptions<T> Execute<T>(Func<CancellationToken, Task<T>> source)
		{
			if (source == null) throw new ArgumentNullException("source");
			return new ObservableViewModelBuilderOptions<T>(_saveViewModel, source);
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source) where TCollection : IEnumerable<T>
		{
			return new UpdatableObservableViewModelBuilderOptions<T, TCollection>(_saveViewModel, source, _backgroundScheduler, _dispatcherScheduler);
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source, T witness) where TCollection : IEnumerable<T>
		{
			return new UpdatableObservableViewModelBuilderOptions<T, TCollection>(_saveViewModel, source, _backgroundScheduler, _dispatcherScheduler);
		}
	}
}