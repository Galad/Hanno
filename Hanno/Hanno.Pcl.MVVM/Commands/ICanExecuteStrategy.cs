using System;
using System.Reactive;

namespace Hanno.Commands
{
	public interface ICanExecuteStrategy<in T>
	{
		void NotifyExecuting(T parameter);
		void NotifyNotExecuting(T parameter);
		bool CanExecute(T parameter);
		IObservable<Unit> CanExecuteChanged { get; }
	}
}