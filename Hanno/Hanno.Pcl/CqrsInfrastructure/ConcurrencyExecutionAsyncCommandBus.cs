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

		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			SemaphoreSlim semaphoreSlim;
			if (_commandTypes.TryGetValue(typeof (TCommand), out semaphoreSlim))
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

		public ConcurrencyExecutionAsyncCommandBus ForCommand<T>(int maximumConcurrentCommands) where T : IAsyncCommand
		{
			_commandTypes.Add(typeof (T), new SemaphoreSlim(maximumConcurrentCommands, maximumConcurrentCommands));
			return this;
		}
	}
}