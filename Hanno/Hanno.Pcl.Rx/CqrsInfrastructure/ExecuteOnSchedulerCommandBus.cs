using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	[DebuggerStepThrough]
	public class ExecuteOnSchedulerCommandBus : ExecuteOnSchedulerBase, IAsyncCommandBus
	{
		private readonly IAsyncCommandBus _commandBus;

		public ExecuteOnSchedulerCommandBus(IAsyncCommandBus commandBus)
		{
			if (commandBus == null) throw new ArgumentNullException("commandBus");
			_commandBus = commandBus;
		}

		public Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			IScheduler scheduler;
			if (Schedulers.TryGetValue(typeof(TCommand), out scheduler))
			{
				return scheduler.Run(() => _commandBus.ProcessCommand(command));
			}
			if (DefaultScheduler != null)
			{
				return DefaultScheduler.Run(() => _commandBus.ProcessCommand(command));
			}
			return _commandBus.ProcessCommand(command);
		}
	}
}