using System;

namespace Hanno.Commands
{
	public class Command<T> : CommandBase<T>
	{
		public Action<T> Action { get; private set; }

		public Command(Action<T> action, ISchedulers schedulers, string name, ICanExecuteStrategy<T> canExecuteStrategy)
			: base(schedulers, name, canExecuteStrategy)
		{
			if (action == null) throw new ArgumentNullException("action");

			Action = action;
		}

		protected override void ExecuteOverride(T parameter)
		{
			Action(parameter);
		}
	}
}