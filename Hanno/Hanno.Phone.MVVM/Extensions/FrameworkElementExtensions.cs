using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Hanno;

namespace System.Windows
{
	public static class FrameworkElementExtensions
	{
		public static IObservable<Unit> ObserveLoaded(this FrameworkElement frameworkElement)
		{
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => frameworkElement.Loaded += h, h => frameworkElement.Loaded -= h)
							 .SelectUnit();
		}

		public static IObservable<Unit> ObserveUnloaded(this FrameworkElement frameworkElement)
		{
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => frameworkElement.Unloaded += h, h => frameworkElement.Unloaded -= h)
							 .SelectUnit();
		}

		public static IScheduler GetDispatcher(this FrameworkElement frameworkElement)
		{
			return new DispatcherScheduler(frameworkElement.Dispatcher);
		}
	}
}