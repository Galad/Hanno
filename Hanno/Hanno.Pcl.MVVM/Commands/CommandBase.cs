using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Hanno.Commands
{
	public abstract class CommandBase<T> : ICommand, IDisposable
	{
		protected readonly string Name;
		private readonly ICanExecuteStrategy<T> _canExecuteStrategy;
		private readonly IDisposable _disposable;

		protected CommandBase(ISchedulers schedulers, string name, ICanExecuteStrategy<T> canExecuteStrategy)
		{
			if (canExecuteStrategy == null) throw new ArgumentNullException("canExecuteStrategy");
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			_disposable = new CompositeDisposable();
			Name = name;
			_canExecuteStrategy = canExecuteStrategy;
			_disposable = _canExecuteStrategy.CanExecuteChanged
			                                 .ObserveOn(schedulers.Dispatcher)
			                                 .Subscribe(b => RaiseCanExecute());
		}

		public bool CanExecute(object parameter)
		{
			return _canExecuteStrategy.CanExecute((T)parameter);
		}

		protected abstract void ExecuteOverride(T parameter);
		public ICanExecuteStrategy<T> CanExecuteStrategy { get { return _canExecuteStrategy; } }

		public void Execute(object parameter)
		{
			ExecuteOverride((T)parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecute()
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		public virtual void Dispose()
		{
			_disposable.Dispose();
		}
	}
}