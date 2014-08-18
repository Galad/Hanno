using System;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class CommandBuilderOptions<T> : ICommandBuilderOptions<T, ICommandBuilderToCommand>
	{
		private readonly Action<ICommand> _saveAction;
		private readonly ISchedulers _schedulers;
		private readonly string _name;
		public Action<T> Action { get; private set; }
		public Func<T, bool> CanExecutePredicate { get; private set; }
		public IObservable<bool> ObservablePredicate { get; private set; }

		public CommandBuilderOptions(Action<T> action, Action<ICommand> saveAction, ISchedulers schedulers, string name)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (saveAction == null) throw new ArgumentNullException("saveAction");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			_saveAction = saveAction;
			_schedulers = schedulers;
			_name = name;
			Action = action;
		}

		public ICommandBuilderToCommand CanExecute(Func<T, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			CanExecutePredicate = predicate;
			ObservablePredicate = null;
			return this;
		}

		public ICommand ToCommand()
		{
			ICanExecuteStrategy<T> canExecuteStrategy;
			if (CanExecutePredicate == null && ObservablePredicate == null)
			{
				canExecuteStrategy = new AlwaysTrueCanExecuteStrategy<T>();
			}
			else if (CanExecutePredicate != null)
			{
				canExecuteStrategy = new SingleExecutionCanExecuteStrategy<T>(CanExecutePredicate);
			}
			else
			{
				canExecuteStrategy = new ObserveCanExecuteStrategy<T>(ObservablePredicate, new AlwaysTrueCanExecuteStrategy<T>());
			}
			ICommand command = new Command<T>(Action, _schedulers, _name, canExecuteStrategy);
			_saveAction(command);
			return command;
		}

		public ICommandBuilderToCommand CanExecute(Func<bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			CanExecutePredicate = _ => predicate();
			ObservablePredicate = null;
			return this;
		}

		public ICommandBuilderToCommand CanExecute(IObservable<bool> observablePredicate)
		{
			if (observablePredicate == null) throw new ArgumentNullException("observablePredicate");
			ObservablePredicate = observablePredicate;
			CanExecutePredicate = null;
			return this;
		}
	}
}