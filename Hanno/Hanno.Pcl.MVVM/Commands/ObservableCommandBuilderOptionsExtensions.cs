using System;
using System.Reactive;

namespace Hanno.Commands
{
	public static class ObservableCommandBuilderOptionsExtensions
    {
        public static IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do<TCommand, TObservable>(
            this IObservableCommandBuilderOptions<TCommand, TObservable> options,
            Action<TObservable> onNext)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (onNext == null) throw new ArgumentNullException("onNext");
			return options.Do(() => Observer.Create(onNext));
        }

        public static IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do<TCommand, TObservable>(
            this IObservableCommandBuilderOptions<TCommand, TObservable> options,
            Action<TObservable> onNext,
            Action onCompleted)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");
			return options.Do(() => Observer.Create(onNext, onCompleted));
        }

        public static IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do<TCommand, TObservable>(
            this IObservableCommandBuilderOptions<TCommand, TObservable> options,
            Action<TObservable> onNext,
            Action<Exception> onError)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            return options.Do(() => Observer.Create(onNext, onError));
        }

        public static IObservableCommandBuilderSchedulerOptions<TCommand, TObservable> Do<TCommand, TObservable>(
            this IObservableCommandBuilderOptions<TCommand, TObservable> options,
            Action<TObservable> onNext,
            Action<Exception> onError,
            Action onCompleted)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");
            return options.Do(() => Observer.Create(onNext, onError, onCompleted));
        }
    }
}