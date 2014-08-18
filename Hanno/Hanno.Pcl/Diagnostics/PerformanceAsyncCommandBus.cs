using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Diagnostics
{
	public class PerformanceAsyncCommandBus : IAsyncCommandBus
	{
		private readonly IAsyncCommandBus _innerCommandBus;

		public PerformanceAsyncCommandBus(IAsyncCommandBus innerCommandBus)
		{
			if (innerCommandBus == null) throw new ArgumentNullException("innerCommandBus");
			_innerCommandBus = innerCommandBus;
		}

		[DebuggerStepThrough]
		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				await _innerCommandBus.ProcessCommand(command);
			}
			finally
			{
				stopwatch.Stop();
				Debug.WriteLine("Command {1} time : {0}", stopwatch.Elapsed, typeof(TCommand));
			}
		}
	}
}
