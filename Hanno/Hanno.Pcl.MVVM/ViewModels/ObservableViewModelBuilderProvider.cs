using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class ObservableViewModelBuilderProvider : IObservableViewModelBuilderProvider, IDisposable
	{
		private readonly Func<IBindable> _getParent;
		private readonly Func<ISchedulers> _getSchedulers;
		private readonly IDictionary<string, IObservableViewModel> viewModels = new Dictionary<string, IObservableViewModel>();
		private readonly CompositeDisposable disposables = new CompositeDisposable();

		public ObservableViewModelBuilderProvider(Func<IBindable> getParent,  Func<ISchedulers> getSchedulers)
		{
			if (getParent == null) throw new ArgumentNullException("getParent");
			if (getSchedulers == null) throw new ArgumentNullException("getSchedulers");
			_getParent = getParent;
			_getSchedulers = getSchedulers;
		}

		public IObservableViewModelBuilder Get(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			IObservableViewModelBuilder builder;
			IObservableViewModel existing;
			bool exist;
			lock (this)
			{
				exist = viewModels.TryGetValue(name, out existing);
				if (!exist)
				{
					viewModels.Add(name, new TemporaryObservableViewModel());
				}
			}
			if (!exist)
			{
				var schedulers = _getSchedulers();
				builder = new ObservableViewModelBuilder(viewModel =>
				{
					viewModels[name] = viewModel;
					var disposable = viewModel as IDisposable;
					if (disposable != null) disposable.DisposeWith(disposables);
					_getParent().NotifyPropertyChanged(name);
				},
					schedulers.ThreadPool,
					schedulers.Dispatcher);
			}
			else
			{
				builder = new ExistingObservableViewModelBuilder(existing);
			}
			return builder;
		}

		public void Dispose()
		{
			disposables.Dispose();
			viewModels.Clear();
		}
	}

	public class TemporaryObservableViewModel : IObservableViewModel
	{
		public IDisposable Subscribe(IObserver<ObservableViewModelNotification> observer)
		{
			return Disposable.Empty;
		}

		public Task RefreshAsync()
		{
			return Task.FromResult(true);
		}

		public void Refresh()
		{
		}
	}
}