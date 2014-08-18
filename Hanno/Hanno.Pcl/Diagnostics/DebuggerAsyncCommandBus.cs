using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Diagnostics
{
	public class DebuggerAsyncCommandBus : IAsyncCommandBus
	{
		private readonly IAsyncCommandBus _innerCommandBus;
		private readonly ConditionalWeakTable<Exception, Exception> _catchedExceptions = new ConditionalWeakTable<Exception, Exception>();

		public DebuggerAsyncCommandBus(IAsyncCommandBus innerCommandBus)
		{
			if (innerCommandBus == null) throw new ArgumentNullException("innerCommandBus");
			_innerCommandBus = innerCommandBus;
		}

		[DebuggerStepThrough]
		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			Debug.WriteLine("[COMMAND] [START] {0}", command);
			try
			{
				await _innerCommandBus.ProcessCommand(command);
				Debug.WriteLine("[COMMAND] [END]   {0}", command);
			}
			catch (Exception ex)
			{
				//prevent writing an exception multiple times
				Exception existing;
				if (!_catchedExceptions.TryGetValue(ex, out existing))
				{
					_catchedExceptions.Add(ex, ex);
					Debug.WriteLine("[COMMAND] [ERROR] {0}\n{1}", command, ex);
				}
				throw;
			}
		}
	}
}