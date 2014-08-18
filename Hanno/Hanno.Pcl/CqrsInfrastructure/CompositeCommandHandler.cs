using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class CompositeCommandHandler<TCommand> : IAsyncCommandHandler<TCommand> where TCommand : IAsyncCommand
	{
		private readonly IEnumerable<IAsyncCommandHandler<TCommand>> _commandHandlers;

		public CompositeCommandHandler(IEnumerable<IAsyncCommandHandler<TCommand>> commandHandlers)
		{
			if (commandHandlers == null) throw new ArgumentNullException("commandHandlers");
			_commandHandlers = commandHandlers;
		}

		public Task Execute(TCommand command)
		{
			return Task.WhenAll(_commandHandlers.Select(c => c.Execute(command)));
		}
	}
}