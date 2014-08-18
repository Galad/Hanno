using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Hanno.Commands
{
	public class AlwaysTrueCanExecuteStrategy<T> : ICanExecuteStrategy<T>
	{
		public void Dispose()
		{
		}

		public void NotifyExecuting(T parameter)
		{
		}

		public void NotifyNotExecuting(T parameter)
		{
		}

		public bool CanExecute(T parameter)
		{
			return true;
		}

		public IObservable<Unit> CanExecuteChanged { get { return Observable.Never<Unit>(); } }
	}
}