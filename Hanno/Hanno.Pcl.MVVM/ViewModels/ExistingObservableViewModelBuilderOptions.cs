using System;
using System.Collections.ObjectModel;

namespace Hanno.ViewModels
{
	public class ExistingObservableViewModelBuilderOptions<T> : IObservableViewModelBuilderOptions<T>
	{
		public IObservableViewModel ViewModel { get; private set; }

		public ExistingObservableViewModelBuilderOptions(IObservableViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			ViewModel = viewModel;
		}

		public IObservableViewModelBuilderOptions<T> EmptyPredicate(Func<T, bool> predicate)
		{
			return this;
		}

		public IObservableViewModelBuilderOptions<T> RefreshOn<TRefresh>(IObservable<TRefresh> refreshTrigger)
		{
			return this;
		}

		public IObservableViewModelBuilderOptions<T> Timeout(TimeSpan timeout)
		{
			return this;
		}

		public IObservableViewModel<T> ToViewModel()
		{
			return (IObservableViewModel<T>)ViewModel;
		}
	}

	public class ExistingUpdatableObservableViewModelBuilderOptionsUpdateOn<T> :
		IUpdatableObservableViewModelBuilderOptionsUpdateOn<T>
	{
		public IObservableViewModel ViewModel { get; private set; }

		public ExistingUpdatableObservableViewModelBuilderOptionsUpdateOn(IObservableViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			ViewModel = viewModel;
		}

		public IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification> UpdateOn<TNotification>(Func<IObservable<TNotification>> notifications)
		{
			return new ExistingUpdatableObservableViewModelBuilderOptionsUpdate<T, TNotification>(ViewModel);
		}
	}

	public class ExistingUpdatableObservableViewModelBuilderOptionsUpdate<T, TNotification> : IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification>
	{
		public IObservableViewModel ViewModel { get; set; }

		public ExistingUpdatableObservableViewModelBuilderOptionsUpdate(IObservableViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			ViewModel = viewModel;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> UpdateAction(Func<TNotification, ObservableCollection<T>, Action> updateActionSelector)
		{
			return new ExistingObservableViewModelBuilderOptions<ObservableCollection<T>>(ViewModel);
		}
	}
}