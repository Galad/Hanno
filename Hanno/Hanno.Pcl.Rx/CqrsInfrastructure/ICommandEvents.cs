using System;

namespace Hanno.CqrsInfrastructure
{
	public interface ICommandEvents
	{
		void NotifyEvent<TCommand, TValue>(TCommand command, TValue eventValue) where TCommand : IAsyncCommandEvent<TValue>;
		IObservable<TValue> ObserveCommandEvents<TCommand, TValue>(TCommand command) where TCommand : IAsyncCommandEvent<TValue>;
		IObservable<TValue> ObserveCommandEvents<TCommand, TValue>() where TCommand : IAsyncCommandEvent<TValue>;
		void NotifyError<TValue>(IAsyncCommandEvent<TValue> command, Exception error);
		void NotifyComplete<TValue>(IAsyncCommandEvent<TValue> command);
	}
}