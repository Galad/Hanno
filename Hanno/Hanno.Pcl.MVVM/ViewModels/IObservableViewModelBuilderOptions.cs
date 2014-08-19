using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hanno.ViewModels
{
	public interface IObservableViewModelBuilderOptions<out T>
	{
		IObservableViewModelBuilderOptions<T> EmptyPredicate(Func<T, bool> predicate);
		IObservableViewModelBuilderOptions<T> RefreshOn<TRefresh>(IObservable<TRefresh> refreshTrigger);
		IObservableViewModelBuilderOptions<T> Timeout(TimeSpan timeout);
		IObservableViewModel<T> ToViewModel();
	}

	public interface IUpdatableObservableViewModelBuilderOptionsUpdateOn<T>
	{
		IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification> UpdateOn<TNotification>(Func<IObservable<TNotification>> notifications);
	}

	public interface IUpdatableObservableViewModelBuilderOptionsUpdateAction<T, TNotification>
	{
		IUpdatableObservableViewModelBuilderOptions<T> UpdateAction(Func<TNotification, ObservableCollection<T>, Action> updateActionSelector);
	}

	public interface IUpdatableObservableViewModelBuilderOptions<T> : IObservableViewModelBuilderOptions<ObservableCollection<T>>
	{
		IObservableViewModelBuilderOptions<ObservableCollection<T>> RefreshOnCollectionUpdateNotification();
	}
}