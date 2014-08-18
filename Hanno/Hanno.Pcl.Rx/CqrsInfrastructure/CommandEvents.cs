using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Hanno.CqrsInfrastructure
{
	public sealed class CommandEvents : ICommandEvents
	{
		private class CommandError
		{
			public object Command;
			public Exception Exception;
		}

		private readonly ISubject<CommandEventArgs> _eventSubject;
		private readonly ISubject<object> _onCompletedSubject;
		private readonly ISubject<CommandError> _onErrorSubject;
		public ISubject<CommandEventArgs> EventSubject { get { return _eventSubject; } }

		public CommandEvents()
		{
			_eventSubject = new Subject<CommandEventArgs>();
			_onCompletedSubject = new Subject<object>();
			_onErrorSubject = new Subject<CommandError>();
		}

		public void NotifyEvent<TCommand, TValue>(TCommand command, TValue eventValue) where TCommand : IAsyncCommandEvent<TValue>
		{
			_eventSubject.OnNext(new CommandEventArgs()
			{
				Command = command,
				Value = eventValue
			});
		}

		public IObservable<TValue> ObserveCommandEvents<TCommand, TValue>(TCommand command) where TCommand : IAsyncCommandEvent<TValue>
		{
			return Observable.Merge(
				_eventSubject.Where(args => args.Command.Equals(command) && args.Value.GetType() == typeof(TValue))
							 .Select(args => (TValue)args.Value)
							 .Select(Notification.CreateOnNext),
				_onCompletedSubject.Where(c => c.Equals(command))
								   .Select(_ => Notification.CreateOnCompleted<TValue>()),
				_onErrorSubject.Where(c => c.Command.Equals(command))
							   .Select(args => Notification.CreateOnError<TValue>(args.Exception))
				)
							 .Dematerialize();
		}

		public IObservable<TValue> ObserveCommandEvents<TCommand, TValue>() where TCommand : IAsyncCommandEvent<TValue>
		{
			return _eventSubject.Where(args => typeof (TCommand).GetTypeInfo().IsAssignableFrom(args.Command.GetType().GetTypeInfo()) &&
			                                   args.Value.GetType() == typeof (TValue))
			                    .Select(args => (TValue) args.Value);
		}

		public void NotifyError<TCommand>(IAsyncCommandEvent<TCommand> command, Exception error)
		{
			_onErrorSubject.OnNext(new CommandError()
			{
				Command = command,
				Exception = error
			});
		}

		public void NotifyComplete<TCommand>(IAsyncCommandEvent<TCommand> command)
		{
			_onCompletedSubject.OnNext(command);
		}
	}
}