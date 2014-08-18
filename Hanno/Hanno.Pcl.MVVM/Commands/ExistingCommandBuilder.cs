using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hanno.Commands
{
    public class ExistingCommandBuilder : ICommandBuilder
    {
        public ICommand Command { get; private set; }

        public ExistingCommandBuilder(ICommand existingCommand)
        {
            if (existingCommand == null) throw new ArgumentNullException("existingCommand");
            Command = existingCommand;
        }

        public ICommandBuilderOptions<ICommandBuilderToCommand> Execute(Action action)
        {
            return new ExistingCommandBuilderOptions<object, object>(Command);
        }

        public ICommandBuilderOptions<T, ICommandBuilderToCommand> Execute<T>(Action<T> action)
        {
            return new ExistingCommandBuilderOptions<T, object>(Command);
        }

        public IObservableCommandBuilderOptions<TCommand, TObservable> Execute<TCommand, TObservable>(Func<TCommand, IObservable<TObservable>> observable)
        {
            return new ExistingCommandBuilderOptions<TCommand, TObservable>(Command);
        }

        public IObservableCommandBuilderOptions<object, TObservable> Execute<TObservable>(Func<IObservable<TObservable>> observable)
        {
            return new ExistingCommandBuilderOptions<object, TObservable>(Command);
        }

	    public IObservableCommandBuilderOptions<TCommand, Unit> Execute<TCommand>(Func<TCommand, CancellationToken, Task> task)
	    {
			return new ExistingCommandBuilderOptions<TCommand, Unit>(Command);
	    }

	    public IObservableCommandBuilderOptions<object, Unit> Execute(Func<CancellationToken, Task> task)
	    {
			return new ExistingCommandBuilderOptions<object, Unit>(Command);
	    }

	    public IObservableCommandBuilderOptions<TCommand, TReturn> Execute<TCommand, TReturn>(Func<TCommand, CancellationToken, Task<TReturn>> observable)
	    {
			return new ExistingCommandBuilderOptions<TCommand, TReturn>(Command);
	    }

	    public IObservableCommandBuilderOptions<object, TReturn> Execute<TReturn>(Func<CancellationToken, Task<TReturn>> observable)
	    {
			return new ExistingCommandBuilderOptions<object, TReturn>(Command);
	    }
    }
}