using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class ExistingCommandBuilderOptions<TCommand, TObservable> : IObservableCommandBuilderOptions<TCommand, TObservable>,
		IObservableCommandBuilderSchedulerOptions<TCommand, TObservable>
	{
		public ICommand Command { get; private set; }

		public ExistingCommandBuilderOptions(ICommand existingCommand)
		{
			if (existingCommand == null) throw new ArgumentNullException("existingCommand");
			Command = existingCommand;
		}


		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(Func<TCommand, bool> predicate)
		{
			return this;
		}

		public IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do(Func<IObserver<TObservable>> observer)
		{
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> Error(Func<CancellationToken, Exception, Task> errorTask)
		{
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> MultipleExecution()
		{
			return this;
		}

		public ICommand ToCommand()
		{
			return Command;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(Func<bool> predicate)
		{
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(IObservable<bool> observablePredicate)
		{
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> WithScheduler(IScheduler scheduler)
		{
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> WithDefaultScheduler()
		{
			return this;
		}
	}
}