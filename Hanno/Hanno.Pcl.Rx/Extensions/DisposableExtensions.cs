using System;
using System.Reactive.Disposables;

namespace System
{
    public static class DisposableExtensions
    {
        public static void DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            if (compositeDisposable == null) throw new ArgumentNullException("compositeDisposable");
            compositeDisposable.Add(disposable);
        }

        public static T DisposeWith<T>(this T obj, CompositeDisposable compositeDisposable)
        {
            var disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.DisposeWith(compositeDisposable);
            }
            return obj;
        }

		public static T DisposeWith<T>(this T obj, SerialDisposable serialDisposable)
		{
			var disposable = obj as IDisposable;
			if (disposable != null)
			{
				serialDisposable.Disposable = disposable;
			}
			return obj;
		}
	}
}