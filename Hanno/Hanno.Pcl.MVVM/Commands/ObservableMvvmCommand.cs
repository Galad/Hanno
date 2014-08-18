using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class ObservableMvvmCommand<TCommand, TObservable> : CommandBase<TCommand>, IMvvmCommand, IAsyncMvvmCommand<TCommand, TObservable>, IAsyncMvvmCommand
	{
		public Func<TCommand, IObservable<TObservable>> Factory { get; private set; }
		public Func<IObserver<TObservable>> DoObserver { get; private set; }
		public IScheduler DoScheduler { get; private set; }
		public Func<CancellationToken, Exception, Task> ErrorTask { get; private set; }
		private readonly bool _hasDefaultErrorTask = false;
		private readonly ReplaySubject<bool> _isExecuting;
		private readonly SerialDisposable _executionDisposable;

		protected readonly CompositeDisposable Disposables = new CompositeDisposable();
		private readonly ISchedulers _schedulers;

		public ObservableMvvmCommand(
			Func<TCommand, IObservable<TObservable>> factory,
			ISchedulers schedulers,
			string name,
			ICanExecuteStrategy<TCommand> canExecuteStrategy,
			Func<IObserver<TObservable>> doObserver = null,
			IScheduler doScheduler = null,
			Func<CancellationToken, Exception, Task> errorTask = null)
			: base(schedulers, name, canExecuteStrategy)
		{
			Factory = factory;
			if (errorTask == null)
			{
				_hasDefaultErrorTask = true;
				errorTask = (ct, ex) => Task.FromResult(true);
			}
			if (doObserver == null)
			{
				doObserver = () => Observer.Create<TObservable>(_ => { });
			}
			DoObserver = doObserver;
			DoScheduler = doScheduler ?? schedulers.Immediate;
			ErrorTask = errorTask;
			_isExecuting = new ReplaySubject<bool>(schedulers.ThreadPool);
			_isExecuting.OnNext(false);
			_schedulers = schedulers;
			_executionDisposable = new SerialDisposable().DisposeWith(Disposables);
		}

		protected override void ExecuteOverride(TCommand parameter)
		{
			CanExecuteStrategy.NotifyExecuting(parameter);
			_schedulers.ThreadPool
			           .Schedule(default(object), (_, s) =>
				           Factory(parameter).ObserveOn(DoScheduler)
				                             .Do(DoObserver())
				                             .Catch((Exception ex) => HandleError(ex))
											 .SubscribeSafe(Observer.Create<TObservable>(value => { }, exception => CanExecuteStrategy.NotifyNotExecuting(parameter), () => CanExecuteStrategy.NotifyNotExecuting(parameter))))
			           .DisposeWith(_executionDisposable);
		}

		private IObservable<TObservable> HandleError(Exception ex)
		{
			return Observable.FromAsync(async (ct) =>
			{
				await ErrorTask(ct, ex);
				return default(TObservable);
			});
		}

		public override void Dispose()
		{
			base.Dispose();
			Disposables.Dispose();
		}

		public void Accept(IMvvmCommandVisitor visitor)
		{
			visitor.Visit((IAsyncMvvmCommand)this);
			visitor.Visit((ICommand)this);
			visitor.Visit((IAsyncMvvmCommand<TCommand, TObservable>)this);
		}

		public bool SetDefaultError(Func<CancellationToken, Exception, Task> errorTask)
		{
			if (errorTask == null) throw new ArgumentNullException("errorTask");
			if (_hasDefaultErrorTask)
			{
				ErrorTask = errorTask;
			}
			return _hasDefaultErrorTask;
		}

		string IAsyncMvvmCommand.Name { get { return Name; } }

		public void DecorateValueFactory(Func<TCommand, IObservable<TObservable>, IObservable<TObservable>> factoryDecorator)
		{
			if (factoryDecorator == null) throw new ArgumentNullException("factoryDecorator");
			var decorated = Factory;
			Factory = arg => factoryDecorator(arg, decorated(arg));
		}

		public void DecorateDo(Func<IObserver<TObservable>, IObserver<TObservable>> factoryDecorator)
		{
			if (factoryDecorator == null) throw new ArgumentNullException("factoryDecorator");
			var decorated = DoObserver;
			DoObserver = () => factoryDecorator(decorated());
		}

		public IObservable<bool> IsExecuting
		{
			get { return _isExecuting; }
		}
	}
}