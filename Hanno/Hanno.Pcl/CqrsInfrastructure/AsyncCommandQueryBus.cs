using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public sealed class AsyncCommandQueryBus : IAsyncCommandBus, IAsyncQueryBus
	{
		private readonly IAsyncCommandHandlerFactory _commandHandlerFactory;
		private readonly IAsyncQueryCommandHandlerFactory _queryCommandHandlerFactory;

		public AsyncCommandQueryBus(
			IAsyncCommandHandlerFactory commandHandlerFactory,
			IAsyncQueryCommandHandlerFactory queryCommandHandlerFactory)
		{
			if (commandHandlerFactory == null) throw new ArgumentNullException("commandHandlerFactory");
			if (queryCommandHandlerFactory == null) throw new ArgumentNullException("queryCommandHandlerFactory");
			_commandHandlerFactory = commandHandlerFactory;
			_queryCommandHandlerFactory = queryCommandHandlerFactory;
		}

		[DebuggerStepThrough]
		public Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			var commandHandler = _commandHandlerFactory.Create<TCommand>();
			if (commandHandler == null)
			{
				throw new InvalidOperationException("Command handler not found for this command");
			}
			return commandHandler.Execute(command);
		}

		[DebuggerStepThrough]
		public Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			var queryHandler = _queryCommandHandlerFactory.Create<TQuery, TResult>();
			if (queryHandler == null)
			{
				throw new InvalidOperationException("Command handler not found for this command");
			}
			return queryHandler.Execute(query);
		}
	}
}