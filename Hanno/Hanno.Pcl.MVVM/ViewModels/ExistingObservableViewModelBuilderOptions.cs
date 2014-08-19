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

		public IUpdatableObservableViewModelBuilderOptions<T> UpdateAction(Func<TNotification, ObservableCollection<T>, Action> updateActionSelector)
		{
			return new ExistingUpdatableObservableViewModelBuilderOptions<T>(ViewModel);
		}
	}

	public class ExistingUpdatableObservableViewModelBuilderOptions<T> : IUpdatableObservableViewModelBuilderOptions<T>
	{
		public ExistingUpdatableObservableViewModelBuilderOptions(IObservableViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			ViewModel = viewModel;
		}

		public IObservableViewModel ViewModel { get; private set; }

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> EmptyPredicate(Func<ObservableCollection<T>, bool> predicate)
		{
			return this;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> RefreshOn<TRefresh>(IObservable<TRefresh> refreshTrigger)
		{
			return this;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> Timeout(TimeSpan timeout)
		{
			return this;
		}

		public IObservableViewModel<ObservableCollection<T>> ToViewModel()
		{
			return (IObservableViewModel<ObservableCollection<T>>)ViewModel;
		}

		public IObservableViewModelBuilderOptions<ObservableCollection<T>> RefreshOnCollectionUpdateNotification()
		{
			return this;
		}
	}
}