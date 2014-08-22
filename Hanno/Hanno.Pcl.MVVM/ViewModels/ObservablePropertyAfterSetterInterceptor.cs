using System;

namespace Hanno.ViewModels
{
	public sealed class ObservablePropertyAfterSetterInterceptor<T> : IObservableProperty<T>
	{
		private readonly IObservableProperty<T> _innerProperty;
		private readonly Action<T, T> _action;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerProperty"></param>
		/// <param name="action">Action to execute after the setter. First parameter is the old value, the second one is the new value.</param>
		public ObservablePropertyAfterSetterInterceptor(IObservableProperty<T> innerProperty, Action<T, T> action)
		{
			if (innerProperty == null) throw new ArgumentNullException("innerProperty");
			if (action == null) throw new ArgumentNullException("action");
			_innerProperty = innerProperty;
			_action = action;
		}
		public void OnCompleted()
		{
			_innerProperty.OnCompleted();
		}

		public void OnError(Exception error)
		{
			_innerProperty.OnError(error);
		}

		public void OnNext(T value)
		{
			var oldValue = _innerProperty.Value;
			_innerProperty.OnNext(value);
			_action(oldValue, value);
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _innerProperty.Subscribe(observer);
		}

		public T Value
		{
			get { return _innerProperty.Value; }
			set
			{
				var oldValue = _innerProperty.Value;
				_innerProperty.Value = value;
				_action(oldValue, value);
			}
		}
	}
}