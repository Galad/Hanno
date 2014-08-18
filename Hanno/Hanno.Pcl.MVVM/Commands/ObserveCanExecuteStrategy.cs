using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hanno.Commands
{
	public class ObserveCanExecuteStrategy<T> : ICanExecuteStrategy<T>
	{
		public IObservable<bool> CanExecuteObservable { get; private set; }
		public readonly ICanExecuteStrategy<T> InnerCanExecuteStrategy;
		private bool _currentCanExecute = true;
		private readonly IDisposable _subscription;
		private readonly Subject<Unit> _observableCanExecuteChanged;

		public ObserveCanExecuteStrategy(IObservable<bool> canExecuteObservable, ICanExecuteStrategy<T> innerCanExecuteStrategy)
		{
			if (canExecuteObservable == null) throw new ArgumentNullException("canExecuteObservable");
			if (innerCanExecuteStrategy == null) throw new ArgumentNullException("innerCanExecuteStrategy");
			CanExecuteObservable = canExecuteObservable;
			InnerCanExecuteStrategy = innerCanExecuteStrategy;
			_observableCanExecuteChanged = new Subject<Unit>();
			_subscription = canExecuteObservable.Subscribe(b =>
			{
				_currentCanExecute = b;
				_observableCanExecuteChanged.OnNext(Unit.Default);
			}, e => { });
		}

		public void NotifyExecuting(T parameter)
		{
			InnerCanExecuteStrategy.NotifyExecuting(parameter);
		}

		public void NotifyNotExecuting(T parameter)
		{
			InnerCanExecuteStrategy.NotifyNotExecuting(parameter);
		}

		public bool CanExecute(T parameter)
		{
			if (!_currentCanExecute)
			{
				return false;
			}
			return InnerCanExecuteStrategy.CanExecute(parameter);
		}

		public IObservable<Unit> CanExecuteChanged
		{
			get
			{
				return InnerCanExecuteStrategy.CanExecuteChanged
											   .Merge(_observableCanExecuteChanged);
			}
		}

		#region

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
				_subscription.Dispose();
				InnerCanExecuteStrategy.Dispose();
				_observableCanExecuteChanged.Dispose();
			}

			//Add disposition of unmanaged resources here

			_isDisposed = true;
		}

		~ObserveCanExecuteStrategy()
		{
			this.Dispose(false);
		}

		private bool _isDisposed;

		#endregion
	}
}