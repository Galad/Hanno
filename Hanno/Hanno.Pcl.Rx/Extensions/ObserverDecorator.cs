using System;
using System.Reactive;

namespace Hanno
{
	public static class ObserverExtensions
	{
		public static IObserver<T> DecorateAfter<T>(this IObserver<T> observer, Action<T> onNext)
		{
			return Observer.Create<T>(value =>
			{
				observer.OnNext(value);
				onNext(value);
			},
			observer.OnError,
			observer.OnCompleted);
		}

		public static IObserver<T> DecorateAfter<T>(this IObserver<T> observer, Action<T> onNext, Action<Exception> onError)
		{
			return Observer.Create<T>(value =>
			{
				observer.OnNext(value);
				onNext(value);
			},
			ex =>
			{
				observer.OnError(ex);
				onError(ex);
			},
			observer.OnCompleted);
		}

		public static IObserver<T> DecorateAfter<T>(this IObserver<T> observer, Action<T> onNext, Action<Exception> onError, Action onComplete)
		{
			return Observer.Create<T>(value =>
			{
				observer.OnNext(value);
				onNext(value);
			},
			ex =>
			{
				observer.OnError(ex);
				onError(ex);
			},
			() =>
			{
				observer.OnCompleted();
				onComplete();
			});
		}

		public static IObserver<T> DecorateBefore<T>(this IObserver<T> observer, Action<T> onNext)
		{
			return Observer.Create<T>(value =>
			{
				onNext(value);
				observer.OnNext(value);
			},
			observer.OnError,
			observer.OnCompleted);
		}

		public static IObserver<T> DecorateBefore<T>(this IObserver<T> observer, Action<T> onNext, Action<Exception> onError)
		{
			return Observer.Create<T>(value =>
			{
				onNext(value);
				observer.OnNext(value);
			},
			ex =>
			{
				onError(ex);
				observer.OnError(ex);
			},
			observer.OnCompleted);
		}

		public static IObserver<T> DecorateBefore<T>(this IObserver<T> observer, Action<T> onNext, Action<Exception> onError, Action onComplete)
		{
			return Observer.Create<T>(value =>
			{
				onNext(value);
				observer.OnNext(value);
			},
			ex =>
			{
				onError(ex);
				observer.OnError(ex);
			},
			() =>
			{
				onComplete();
				observer.OnCompleted();
			});
		}
	}
}