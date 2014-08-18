using System;
using System.Diagnostics.Contracts;
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
    }
}