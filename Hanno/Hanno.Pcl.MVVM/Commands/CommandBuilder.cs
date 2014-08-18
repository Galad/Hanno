using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class CommandBuilder : ICommandBuilder
	{
		private readonly Action<ICommand> _saveAction;
		private readonly ISchedulers _schedulers;
		private readonly string _name;

		public CommandBuilder(Action<ICommand> saveAction, ISchedulers schedulers, string name)
		{
			if (saveAction == null) throw new ArgumentNullException("saveAction");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			_saveAction = saveAction;
			_schedulers = schedulers;
			_name = name;
		}

		public ICommandBuilderOptions<ICommandBuilderToCommand> Execute(Action action)
		{
			return new CommandBuilderOptions<object>(_ => action(), _saveAction, _schedulers, _name);
		}

		public ICommandBuilderOptions<T, ICommandBuilderToCommand> Execute<T>(Action<T> action)
		{
			return new CommandBuilderOptions<T>(action, _saveAction, _schedulers, _name);
		}

		public IObservableCommandBuilderOptions<TCommand, TObservable> Execute<TCommand, TObservable>(Func<TCommand, IObservable<TObservable>> observable)
		{
			return new ObservableCommandBuilderOptions<TCommand, TObservable>(observable, _saveAction, _schedulers, _name);
		}

		public IObservableCommandBuilderOptions<object, TObservable> Execute<TObservable>(Func<IObservable<TObservable>> observable)
		{
			return new ObservableCommandBuilderOptions<object, TObservable>(o => observable(), _saveAction, _schedulers, _name);
		}

		public IObservableCommandBuilderOptions<TCommand, Unit> Execute<TCommand>(Func<TCommand, CancellationToken, Task> task)
		{
			return new ObservableCommandBuilderOptions<TCommand, Unit>(
				o => Observable.FromAsync(ct => task(o, ct)),
				_saveAction,
				_schedulers,
				_name);
		}

		public IObservableCommandBuilderOptions<object, Unit> Execute(Func<CancellationToken, Task> task)
		{
			return new ObservableCommandBuilderOptions<object, Unit>(
				_ => Observable.FromAsync(task),
				_saveAction,
				_schedulers,
				_name);
		}

		public IObservableCommandBuilderOptions<TCommand, TReturn> Execute<TCommand, TReturn>(Func<TCommand, CancellationToken, Task<TReturn>> task)
		{
			return new ObservableCommandBuilderOptions<TCommand, TReturn>(
					o => Observable.FromAsync(ct => task(o, ct)),
					_saveAction,
					_schedulers,
					_name);
		}

		public IObservableCommandBuilderOptions<object, TReturn> Execute<TReturn>(Func<CancellationToken, Task<TReturn>> task)
		{
			return new ObservableCommandBuilderOptions<object, TReturn>(
				_ => Observable.FromAsync(task),
				_saveAction,
				_schedulers,
				_name);
		}
	}
}