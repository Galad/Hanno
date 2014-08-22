using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Validation;

namespace Hanno.ViewModels
{
	public interface IObservableProperty<T> : ISubject<T>
	{
		T Value { get; set; }
	}

	public class ObservableProperty<T> : Validable, IObservableProperty<T>
	{
		private bool _hasValue = false;

		public ObservableProperty(ISchedulers schedulers)
			: base(schedulers)
		{
		}

		public T Value
		{
			get { return GetValue<T>(); }
			set
			{
				lock (this)
				{
					_hasValue = true;
					SetValue(value);
				}
			}
		}

		public Exception Error
		{
			get { return GetValue<Exception>(); }
			private set
			{
				SetValue(value);
			}
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
			Error = error;
		}

		public void OnNext(T value)
		{
			lock (this)
			{
				_hasValue = true;
				Value = value;
			}
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return this.ObserveProperty(() => Value)
					   .StartWith(ImmediateScheduler.Instance, Value)
					   .Where(_ => _hasValue)
					   .Subscribe(observer);
		}
	}

	public static class ObservablePropertyExtensions
	{
		public static AsyncSubject<T> GetAwaiter<T>(this IObservableProperty<T> observableProperty)
		{
			return observableProperty.Take(1).GetAwaiter();
		}

		public static IObservableProperty<T> InterceptSetterBefore<T>(this IObservableProperty<T> observableProperty, Action<T, T> interceptor)
		{
			if (interceptor == null) throw new ArgumentNullException("interceptor");
			return new ObservablePropertyBeforeSetterInterceptor<T>(observableProperty, interceptor);
		}

		public static IObservableProperty<T> InterceptSetterAfter<T>(this IObservableProperty<T> observableProperty, Action<T, T> interceptor)
		{
			if (interceptor == null) throw new ArgumentNullException("interceptor");
			return new ObservablePropertyAfterSetterInterceptor<T>(observableProperty, interceptor);
		}
	}
}
