using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class ObservableViewModelBuilderProvider : IObservableViewModelBuilderProvider, IDisposable
	{
		private readonly Func<IBindable> _getParent;
		private readonly Func<ISchedulers> _getSchedulers;
		private readonly Func<Action<IObservableViewModel>, IScheduler, IScheduler, IObservableViewModelBuilder> _builderFactory;
		private readonly IDictionary<string, IObservableViewModel> viewModels = new Dictionary<string, IObservableViewModel>();
		private readonly CompositeDisposable disposables = new CompositeDisposable();
		private List<IObservableViewModelVisitor> _visitors;

		public ObservableViewModelBuilderProvider(
			Func<IBindable> getParent,
			Func<ISchedulers> getSchedulers,
			Func<Action<IObservableViewModel>, IScheduler, IScheduler, IObservableViewModelBuilder> builderFactory)
		{
			if (getParent == null) throw new ArgumentNullException("getParent");
			if (getSchedulers == null) throw new ArgumentNullException("getSchedulers");
			if (builderFactory == null) throw new ArgumentNullException("builderFactory");
			_getParent = getParent;
			_getSchedulers = getSchedulers;
			_builderFactory = builderFactory;
			_visitors = new List<IObservableViewModelVisitor>();
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
				builder = _builderFactory(viewModel =>
				{
					foreach (var observableViewModelVisitor in _visitors)
					{
						viewModel.Accept(observableViewModelVisitor);
					}
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

		public void AddVisitor(IObservableViewModelVisitor visitor)
		{
			_visitors.Add(visitor);
		}

		public void CopyVisitors(IObservableViewModelBuilderProvider observableViewModelBuilderProvider)
		{
			foreach (var observableViewModelVisitor in _visitors)
			{
				observableViewModelBuilderProvider.AddVisitor(observableViewModelVisitor);
			}
		}

		public void Dispose()
		{
			disposables.Dispose();
			viewModels.Clear();
		}
	}

	internal class TemporaryObservableViewModel : IObservableViewModel
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

		public void Accept(IObservableViewModelVisitor visitor)
		{
		}
	}
}