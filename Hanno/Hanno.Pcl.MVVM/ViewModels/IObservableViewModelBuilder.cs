using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
	public interface IObservableViewModelBuilder
	{
		IObservableViewModelBuilderOptions<T> Execute<T>(Func<CancellationToken, Task<T>> source);
		IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source) where TCollection : IEnumerable<T>;
		/// <summary>
		/// Set the execute task for this ObservableViewModel, using an enumerable as the result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCollection"></typeparam>
		/// <param name="source"></param>
		/// <param name="witness"></param>
		/// <returns></returns>
		IUpdatableObservableViewModelBuilderOptionsUpdateOn<T> ExecuteUpdatable<T, TCollection>(Func<CancellationToken, Task<TCollection>> source, T witness) where TCollection : IEnumerable<T>;
	}
}