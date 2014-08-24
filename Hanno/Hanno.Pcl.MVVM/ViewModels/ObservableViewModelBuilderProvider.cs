using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class ObservableViewModelBuilderProvider : IObservableViewModelBuilderProvider, IDisposable
	{
		private readonly Func<ISchedulers> _getSchedulers;
		private readonly Func<Action<IObservableViewModel>, IScheduler, IScheduler, IObservableViewModelBuilder> _builderFactory;
		private readonly CompositeDisposable _disposables = new CompositeDisposable();
		private readonly List<IObservableViewModelVisitor> _visitors;

		public ObservableViewModelBuilderProvider(
			Func<ISchedulers> getSchedulers,
			Func<Action<IObservableViewModel>, IScheduler, IScheduler, IObservableViewModelBuilder> builderFactory)
		{
			if (getSchedulers == null) throw new ArgumentNullException("getSchedulers");
			if (builderFactory == null) throw new ArgumentNullException("builderFactory");
			_getSchedulers = getSchedulers;
			_builderFactory = builderFactory;
			_visitors = new List<IObservableViewModelVisitor>();
		}

		public IObservableViewModelBuilder Get(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

			var schedulers = _getSchedulers();
			var builder = _builderFactory(viewModel =>
			{
				foreach (var observableViewModelVisitor in _visitors)
				{
					viewModel.Accept(observableViewModelVisitor);
				}
				var disposable = viewModel as IDisposable;
				if (disposable != null) disposable.DisposeWith(_disposables);
			},
				schedulers.ThreadPool,
				schedulers.Dispatcher);
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
			_disposables.Dispose();
		}
	}
}