using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class ConcurrencyExecutionAsyncCommandBus : IAsyncCommandBus
	{
		private readonly IAsyncCommandBus _innerCommandBus;
		private readonly Dictionary<Type, SemaphoreSlim> _commandTypes = new Dictionary<Type, SemaphoreSlim>();

		public ConcurrencyExecutionAsyncCommandBus(IAsyncCommandBus innerCommandBus)
		{
			if (innerCommandBus == null) throw new ArgumentNullException("innerCommandBus");
			_innerCommandBus = innerCommandBus;
		}

		public Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			return ProcessCommandInternal(command);
		}

		private async Task ProcessCommandInternal<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			SemaphoreSlim semaphoreSlim;
			if (_commandTypes.TryGetValue(typeof(TCommand), out semaphoreSlim))
			{
				await semaphoreSlim.WaitAsync(command.CancellationToken);
				try
				{
					await _innerCommandBus.ProcessCommand(command);
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
			else
			{
				await _innerCommandBus.ProcessCommand(command);
			}
		}

		public ConcurrencyExecutionAsyncCommandBus ForCommand<T>(ushort maximumConcurrentCommands) where T : IAsyncCommand
		{
			_commandTypes.Add(typeof (T), new SemaphoreSlim(maximumConcurrentCommands, maximumConcurrentCommands));
			return this;
		}
	}
}