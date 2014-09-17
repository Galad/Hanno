using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Commands
{
	public class ObservableMvvmCommandWithObservableCanExecute<TCommand, TObservable> : ObservableMvvmCommand<TCommand, TObservable>
	{
		public ObservableMvvmCommandWithObservableCanExecute(
			Func<TCommand, IObservable<TObservable>> factory,
			ISchedulers schedulers,
			IScheduler executionScheduler,
			string name,
			IObservable<bool> observablePredicate,
			ICanExecuteStrategy<TCommand> canExecuteStrategy,
			Func<IObserver<TObservable>> doObserver = null,
			IScheduler doScheduler = null,
			Func<CancellationToken, Exception, Task> errorTask = null)
			: base(
				factory,
				schedulers,
				executionScheduler,
				name,
				new ObserveCanExecuteStrategy<TCommand>(observablePredicate, canExecuteStrategy),
				doObserver,
				doScheduler,
				errorTask)
		{
		}
	}
}