using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace Hanno.Commands
{
	public class SingleExecutionCanExecuteStrategy<T> : ICanExecuteStrategy<T>, IDisposable
	{
		public readonly Func<T, bool> CanExecutePredicate;
		private readonly Subject<Unit> _canExecuteChanged;
		private bool _isExecuting;

		public SingleExecutionCanExecuteStrategy(Func<T, bool> canExecutePredicate)
		{
			if (canExecutePredicate == null) throw new ArgumentNullException("canExecutePredicate");
			CanExecutePredicate = canExecutePredicate;
			_canExecuteChanged = new Subject<Unit>();
		}

		public void NotifyExecuting(T parameter)
		{
			_isExecuting = true;
			_canExecuteChanged.OnNext(Unit.Default);
		}

		public void NotifyNotExecuting(T parameter)
		{
			_isExecuting = false;
			_canExecuteChanged.OnNext(Unit.Default);
		}

		public bool CanExecute(T parameter)
		{
			return !_isExecuting && CanExecutePredicate(parameter);
		}

		public IObservable<Unit> CanExecuteChanged { get { return _canExecuteChanged; } }


		#region Dispose
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				_canExecuteChanged.Dispose();
			}

			//Add disposition of unmanaged resources here

			_isDisposed = true;
		}

		~SingleExecutionCanExecuteStrategy()
		{
			this.Dispose(false);
		}

		private bool _isDisposed;

		#endregion


	}
}