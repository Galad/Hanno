using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Commands
{
	public interface ICommandBuilder
	{
		ICommandBuilderOptions<ICommandBuilderToCommand> Execute(Action action);
		ICommandBuilderOptions<T, ICommandBuilderToCommand> Execute<T>(Action<T> action);
		IObservableCommandBuilderOptions<TCommand, TObservable> Execute<TCommand, TObservable>(Func<TCommand, IObservable<TObservable>> observable);
		IObservableCommandBuilderOptions<object, TObservable> Execute<TObservable>(Func<IObservable<TObservable>> observable);
		IObservableCommandBuilderOptions<TCommand, Unit> Execute<TCommand>(Func<TCommand, CancellationToken, Task> task);
		IObservableCommandBuilderOptions<object, Unit> Execute(Func<CancellationToken, Task> task);
		IObservableCommandBuilderOptions<TCommand, TReturn> Execute<TCommand, TReturn>(Func<TCommand, CancellationToken, Task<TReturn>> observable);
		IObservableCommandBuilderOptions<object, TReturn> Execute<TReturn>(Func<CancellationToken, Task<TReturn>> observable);
	}
}