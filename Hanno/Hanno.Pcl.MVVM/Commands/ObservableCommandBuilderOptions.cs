using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class ObservableCommandBuilderOptions<TCommand, TObservable> : IObservableCommandBuilderOptions<TCommand, TObservable>,
		IObservableCommandBuilderSchedulerOptions<TCommand, TObservable>
	{
		private readonly Action<ICommand> _saveAction;
		private readonly ISchedulers _schedulers;
		private readonly string _name;
		private Action<IScheduler> _setScheduler;
		public Func<TCommand, IObservable<TObservable>> Observable { get; private set; }
		public Func<TCommand, bool> CanExecutePredicate { get; private set; }
		public IObservable<bool> ObservableCanExecute { get; private set; }
		public Func<IObserver<TObservable>> DoObserver { get; private set; }
		public IScheduler DoScheduler { get; private set; }
		public Func<CancellationToken, Exception, Task> ErrorTask { get; set; }
		public bool IsMultipleExecution { get; private set; }

		public ObservableCommandBuilderOptions(Func<TCommand, IObservable<TObservable>> observable, Action<ICommand> saveAction, ISchedulers schedulers, string name)
		{
			if (observable == null) throw new ArgumentNullException("observable");
			if (saveAction == null) throw new ArgumentNullException("saveAction");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			_saveAction = saveAction;
			_schedulers = schedulers;
			_name = name;
			Observable = observable;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(Func<TCommand, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			CanExecutePredicate = predicate;
			ObservableCanExecute = null;
			return this;
		}

		public IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do(Func<IObserver<TObservable>> observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");
			_setScheduler = scheduler => DoScheduler = scheduler;
			DoObserver = observer;
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> Error(Func<CancellationToken, Exception, Task> errorTask)
		{
			if (errorTask == null) throw new ArgumentNullException("errorTask");
			ErrorTask = errorTask;
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> MultipleExecution()
		{
			IsMultipleExecution = true;
			return this;
		}

		public ICommand ToCommand()
		{
			ICommand command;
			ICanExecuteStrategy<TCommand> canExecuteStrategy;
			if (CanExecutePredicate == null && ObservableCanExecute == null)
			{
				canExecuteStrategy = new SingleExecutionCanExecuteStrategy<TCommand>(_ => true);
			}
			else if (CanExecutePredicate != null)
			{
				if (IsMultipleExecution)
				{
					canExecuteStrategy = new MultipleExecutionCanExecuteStrategy<TCommand>(CanExecutePredicate);
				}
				else
				{
					canExecuteStrategy = new SingleExecutionCanExecuteStrategy<TCommand>(CanExecutePredicate);
				}
			}
			else
			{
				canExecuteStrategy = IsMultipleExecution ?
					new ObserveCanExecuteStrategy<TCommand>(ObservableCanExecute, new MultipleExecutionCanExecuteStrategy<TCommand>(_ => true)) :
					new ObserveCanExecuteStrategy<TCommand>(ObservableCanExecute, new SingleExecutionCanExecuteStrategy<TCommand>(_ => true));
			}
			command = new ObservableMvvmCommand<TCommand, TObservable>(
				Observable,
				_schedulers,
				_name,
				canExecuteStrategy,
				DoObserver,
				DoScheduler,
				ErrorTask);
			_saveAction(command);
			return command;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(Func<bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			CanExecutePredicate = _ => predicate();
			ObservableCanExecute = null;
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> CanExecute(IObservable<bool> observablePredicate)
		{
			if (observablePredicate == null) throw new ArgumentNullException("observablePredicate");
			ObservableCanExecute = observablePredicate;
			CanExecutePredicate = null;
			return this;
		}


		public IObservableCommandBuilderOptions<TCommand, TObservable> WithScheduler(IScheduler scheduler)
		{
			_setScheduler(scheduler);
			return this;
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> WithDefaultScheduler()
		{
			_setScheduler(_schedulers.Immediate);
			return this;
		}
	}
}