using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive.Linq
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> SelectUnit<T>(this IObservable<T> source)
        {
            return source.Select(s => Unit.Default);
        }

	    public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<T, CancellationToken, Task> task)
	    {
		    return source.SelectMany(async (arg1, token) =>
		    {
			    await task(arg1, token);
			    return Unit.Default;
		    });
	    }

	    public static IObservable<T> ReplayAndConnect<T>(this IObservable<T> source, int count, CompositeDisposable disposables, IScheduler scheduler = null)
	    {
		    if (source == null) throw new ArgumentNullException("source");
		    IConnectableObservable<T> replay;
		    if (scheduler == null)
		    {
			    replay = source.Replay(count);
		    }
		    else
		    {
			    replay = source.Replay(count, scheduler);
		    }
		    replay.Connect().DisposeWith(disposables);
		    return replay;
	    }

		/// <summary>
		/// Apply a selector on the previous value produced by an observable and the current value
		/// </summary>
		/// <typeparam name="TSource">Type of the source</typeparam>
		/// <typeparam name="TDelta">Type of the value returned by the delta selector</typeparam>
		/// <param name="source">Observable source</param>
		/// <param name="deltaSelector">Select the delta between the previous value and the current value.
		/// The first parameter is the previous value.
		/// The second parameter is the current value.</param>
		/// <returns></returns>
	    public static IObservable<TDelta> Delta<TSource, TDelta>(this IObservable<TSource> source, Func<TSource, TSource, TDelta> deltaSelector)
	    {
		    return source.Scan(new TSource[] {}, (previousValues, newValue) =>
		    {
			    if (previousValues.Length == 0)
			    {
				    return new[] {newValue};
			    }
			    if (previousValues.Length == 1)
			    {
				    return new[] {previousValues[0], newValue};
			    }
			    return new[] {previousValues[1], newValue};
		    })
		                 .Where(values => values.Length == 2)
		                 .Select(values => deltaSelector(values[0], values[1]));
	    }
    }
}