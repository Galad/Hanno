using System;
using System.Reactive.Linq;

namespace Hanno.Commands
{
	public class CommandWithObservableCanExecute<T> : Command<T>
	{
		public CommandWithObservableCanExecute(Action<T> action, IObservable<bool> observablePredicate, ISchedulers schedulers, string name, ICanExecuteStrategy<T> canExecuteStrategy)
			: base(action, schedulers, name, new ObserveCanExecuteStrategy<T>(observablePredicate, canExecuteStrategy))
		{
		}
	}
}