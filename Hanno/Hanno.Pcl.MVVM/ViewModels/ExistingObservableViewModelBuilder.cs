using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public class ExistingObservableViewModelBuilder : IObservableViewModelBuilder
	{
		public IObservableViewModel ViewModel { get; private set; }

		public ExistingObservableViewModelBuilder(IObservableViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			ViewModel = viewModel;
		}

		public IObservableViewModelBuilderOptions<T2> Execute<T2>(Func<CancellationToken, Task<T2>> source)
		{
			return new ExistingObservableViewModelBuilderOptions<T2>(ViewModel);
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source) where TCollection : IEnumerable<T>
		{
			return ExecuteUpdatable(source, default(T));
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source, T witness) where TCollection : IEnumerable<T>
		{
			return new ExistingUpdatableObservableViewModelBuilderOptionsUpdateOn<T>(ViewModel);
		}
	}
}