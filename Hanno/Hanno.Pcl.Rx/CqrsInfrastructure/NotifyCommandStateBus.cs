using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class NotifyCommandStateBus : IAsyncCommandBus, ICommandStateEvents
	{
		private readonly IAsyncCommandBus _commandBus;
		private readonly IScheduler _scheduler;
		private readonly Subject<IAsyncCommand> _commandStarted = new Subject<IAsyncCommand>();
		private readonly Subject<IAsyncCommand> _commandEnded = new Subject<IAsyncCommand>();
		private readonly Subject<Tuple<IAsyncCommand, Exception>> _commandError = new Subject<Tuple<IAsyncCommand, Exception>>();
		private readonly List<IAsyncCommand> _executingCommands;


		public NotifyCommandStateBus(
			IAsyncCommandBus commandBus,
			IScheduler scheduler)
		{
			if (commandBus == null) throw new ArgumentNullException("commandBus");
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			_commandBus = commandBus;
			_scheduler = scheduler;
			_executingCommands = new List<IAsyncCommand>();
		}

		[DebuggerStepThrough]
		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			try
			{
				lock (this)
				{
					_executingCommands.Add(command);
				}
				_commandStarted.OnNext(command);
				await _commandBus.ProcessCommand(command);
				_commandEnded.OnNext(command);
			}
			catch (Exception ex)
			{
				_commandError.OnNext(new Tuple<IAsyncCommand, Exception>(command, ex));
				throw;
			}
			finally
			{
				lock (this)
				{
					_executingCommands.Remove(command);
				}
			}
		}

		#region CommandStateEvents
		public IObservable<IAsyncCommand> ObserveCommandStarted()
		{
			lock (this)
			{
				return _commandStarted.StartWith(_scheduler, _executingCommands);
			}
		}

		public IObservable<IAsyncCommand> ObserveCommandEnded()
		{
			return _commandEnded;
		}

		public IObservable<Tuple<IAsyncCommand, Exception>> ObserveCommandError()
		{
			return _commandError;
		}


		#endregion

	}
}