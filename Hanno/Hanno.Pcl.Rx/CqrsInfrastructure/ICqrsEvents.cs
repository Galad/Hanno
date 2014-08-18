using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Hanno.Extensions;

namespace Hanno.CqrsInfrastructure
{
	public interface ICommandStateEvents
	{
		IObservable<IAsyncCommand> ObserveCommandStarted();
		IObservable<IAsyncCommand> ObserveCommandEnded();
		IObservable<Tuple<IAsyncCommand, Exception>> ObserveCommandError();
	}

	public interface IQueryStateEvents
	{
		IObservable<IAsyncQuery<object>> ObserveQueryStarted();
		IObservable<Tuple<IAsyncQuery<object>, object>> ObserveQueryEnded();
		IObservable<Tuple<IAsyncQuery<object>, Exception>> ObserveQueryError();
	}

	public static class CqrsEventsEventsExtensions
	{
		public static IObservable<TCommand> ObserveCommandStarted<TCommand>(this ICommandStateEvents events) where TCommand : IAsyncCommand
		{
			return events.ObserveCommandStarted()
						 .Where(command => command.IsAssignableTo<TCommand>())
						 .Select(command => (TCommand)command);
		}

		public static IObservable<TCommand> ObserveCommandEnded<TCommand>(this ICommandStateEvents events) where TCommand : IAsyncCommand
		{
			return events.ObserveCommandEnded()
						 .Where(command => command.IsAssignableTo<TCommand>())
						 .Select(command => (TCommand)command);
		}

		public static IObservable<Tuple<TCommand, Exception>> ObserveCommandError<TCommand>(this ICommandStateEvents events) where TCommand : IAsyncCommand
		{
			return events.ObserveCommandError()
						 .Where(tuple => tuple.Item1.IsAssignableTo<TCommand>())
						 .Select(tuple => new Tuple<TCommand, Exception>((TCommand)tuple.Item1, tuple.Item2));
		}

		public static IObservable<TQuery> ObserveQueryStarted<TQuery>(this IQueryStateEvents events) where TQuery : IAsyncQuery<object>
		{
			return events.ObserveQueryStarted()
						 .Where(query => query.IsAssignableTo<TQuery>())
						 .Select(query => (TQuery)query);
		}

		public static IObservable<Tuple<TQuery, TValue>> ObserveQueryEnded<TQuery, TValue>(this IQueryStateEvents events) where TQuery : IAsyncQuery<TValue>
		{
			return events.ObserveQueryEnded()
						 .Where(tuple => tuple.Item1.IsAssignableTo<TQuery>())
						 .Select(query => new Tuple<TQuery, TValue>((TQuery)query.Item1, (TValue)query.Item2));
		}

		public static IObservable<Tuple<TQuery, Exception>> ObserveQueryError<TQuery>(this IQueryStateEvents events) where TQuery : IAsyncQuery<object>
		{
			return events.ObserveQueryError()
						 .Where(tuple => tuple.Item1.IsAssignableTo<TQuery>())
						 .Select(tuple => new Tuple<TQuery, Exception>((TQuery)tuple.Item1, tuple.Item2));
		}

		public static IObservable<bool> ObserveCommandIsExecuting<TCommand>(this ICommandStateEvents events) where TCommand : IAsyncCommand
		{
			return Observable.Merge(
				events.ObserveCommandStarted<TCommand>().Select(_ => true),
				events.ObserveCommandEnded<TCommand>().Select(_ => false),
				events.ObserveCommandError<TCommand>().Select(_ => false)
				);
		}

		public static IObservable<bool> ObserveCommandIsExecuting<TCommand>(this ICommandStateEvents events, TCommand command) where TCommand : IAsyncCommand
		{
			return Observable.Merge(
				events.ObserveCommandStarted<TCommand>().Where(c => EqualityComparer<TCommand>.Default.Equals(command, c)).Select(_ => true),
				events.ObserveCommandEnded<TCommand>().Where(c => EqualityComparer<TCommand>.Default.Equals(command, c)).Select(_ => false),
				events.ObserveCommandError<TCommand>().Where(c => EqualityComparer<TCommand>.Default.Equals(command, c.Item1)).Select(_ => false)
				);
		}
	}
}