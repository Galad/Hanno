using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Commands
{
	public interface IObservableCommandBuilderOptions<out TCommand, out TObservable> : ICommandBuilderOptions<TCommand, IObservableCommandBuilderOptions<TCommand, TObservable>>
	{
		IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do(Func<IObserver<TObservable>> observer);
		IObservableCommandBuilderOptions<TCommand, TObservable> Error(Func<CancellationToken, Exception, Task> errorTask);
		IObservableCommandBuilderOptions<TCommand, TObservable> MultipleExecution();
		IObservableCommandBuilderOptions<TCommand, TObservable> ExecuteOnScheduler(IScheduler scheduler);
	}
}