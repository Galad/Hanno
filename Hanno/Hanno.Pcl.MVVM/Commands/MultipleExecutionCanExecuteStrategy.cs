using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Hanno.Commands
{
	/// <summary>
	/// Handle CanExecute values separatly for each parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MultipleExecutionCanExecuteStrategy<T> : ICanExecuteStrategy<T>
	{
		private class Execution
		{
			public bool IsExecuting;
		}
		
		public Func<T, bool> CanExecutePredicate { get; private set; }

		private readonly Subject<Unit> _canExecuteChanged;
		private readonly IDictionary<object, Execution> _executions;
		private readonly object _defaultParameter = new object();

		public MultipleExecutionCanExecuteStrategy(Func<T, bool> canExecutePredicate)
		{
			if (canExecutePredicate == null) throw new ArgumentNullException("canExecutePredicate");
			CanExecutePredicate = canExecutePredicate;
			_canExecuteChanged = new Subject<Unit>();
			_executions = new Dictionary<object, Execution>();
		}

		public void NotifyExecuting(T parameter)
		{
			ChangeExecutionState(parameter, true);
			_canExecuteChanged.OnNext(Unit.Default);
		}

		private void ChangeExecutionState(T parameter, bool b)
		{
			Execution execution ;
			var key = EqualityComparer<T>.Default.Equals(default(T), parameter) ? _defaultParameter : parameter;
			if (! _executions.TryGetValue(key, out execution))
			{
				execution = new Execution() {IsExecuting = b};
				_executions.Add(key, execution);
				return;
			}
			execution.IsExecuting = b;
		}

		public void NotifyNotExecuting(T parameter)
		{
			ChangeExecutionState(parameter, false);
			_canExecuteChanged.OnNext(Unit.Default);
		}

		public bool CanExecute(T parameter)
		{
			Execution execution;
			bool isExecuting = false;
			var key = EqualityComparer<T>.Default.Equals(default(T), parameter) ? _defaultParameter : parameter;
			if (_executions.TryGetValue(key, out execution))
			{
				isExecuting = execution.IsExecuting;
			}
			return !isExecuting && CanExecutePredicate(parameter);
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

		~MultipleExecutionCanExecuteStrategy()
		{
			this.Dispose(false);
		}

		private bool _isDisposed;
		#endregion


	}
}