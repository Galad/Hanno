using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class NotifyCommandStateBus : IAsyncCommandBus, ICommandStateEvents
	{
		private readonly IAsyncCommandBus _commandBus;
		private readonly Subject<IAsyncCommand> _commandStarted = new Subject<IAsyncCommand>();
		private readonly Subject<IAsyncCommand> _commandEnded = new Subject<IAsyncCommand>();
		private readonly Subject<Tuple<IAsyncCommand, Exception>> _commandError = new Subject<Tuple<IAsyncCommand, Exception>>();


		public NotifyCommandStateBus(
			IAsyncCommandBus commandBus)
		{
			if (commandBus == null) throw new ArgumentNullException("commandBus");
			_commandBus = commandBus;
		}

		[DebuggerStepThrough]
		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			try
			{
				_commandStarted.OnNext(command);
				await _commandBus.ProcessCommand(command);
				_commandEnded.OnNext(command);
			}
			catch (Exception ex)
			{
				_commandError.OnNext(new Tuple<IAsyncCommand, Exception>(command, ex));
				throw;
			}
		}

		#region CommandStateEvents
		public IObservable<IAsyncCommand> ObserveCommandStarted()
		{
			return _commandStarted;
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