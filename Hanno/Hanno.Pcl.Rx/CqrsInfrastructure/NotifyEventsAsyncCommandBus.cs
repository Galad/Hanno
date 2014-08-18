using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class NotifyEventsAsyncCommandBus : IAsyncCommandBus
	{
		private readonly IAsyncCommandBus _innerAsyncCommandBus;
		private readonly ICommandEvents _commandEvents;

		public NotifyEventsAsyncCommandBus(IAsyncCommandBus innerAsyncCommandBus, ICommandEvents commandEvents)
		{
			if (innerAsyncCommandBus == null) throw new ArgumentNullException("innerAsyncCommandBus");
			if (commandEvents == null) throw new ArgumentNullException("commandEvents");
			_innerAsyncCommandBus = innerAsyncCommandBus;
			_commandEvents = commandEvents;
		}

		[DebuggerStepThrough]
		public async Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			var eventType = command.GetType().GetTypeInfo()
				.ImplementedInterfaces
				.FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncCommandEvent<>));
			if (eventType != null)
			{
				eventType = eventType.GetTypeInfo().GenericTypeArguments.First();
			}

			if (eventType == null)
			{
				await _innerAsyncCommandBus.ProcessCommand(command);
				return;
			}
			try
			{
				await _innerAsyncCommandBus.ProcessCommand(command);
				var method = typeof(ICommandEvents).GetTypeInfo().GetDeclaredMethod("NotifyComplete");
				method = method.MakeGenericMethod(eventType);
				method.Invoke(_commandEvents, new object[] { command });
			}
			catch (Exception ex)
			{
				var method = typeof(ICommandEvents).GetTypeInfo().GetDeclaredMethod("NotifyError");
				method = method.MakeGenericMethod(eventType);
				method.Invoke(_commandEvents, new object[] { command, ex });
				throw;
			}
		}
	}
}