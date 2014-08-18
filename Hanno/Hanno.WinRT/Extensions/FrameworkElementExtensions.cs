using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Hanno;

namespace Windows.UI.Xaml
{
	public static class FrameworkElementExtensions
	{
		public static IScheduler GetDispatcher(this FrameworkElement frameworkElement)
		{
			if (!DesignMode.DesignModeEnabled)
			{
				return new CoreDispatcherScheduler(frameworkElement.Dispatcher);
			}
			else
			{
				return Scheduler.CurrentThread;
			}
		}

		public static IObservable<Unit> ObserveLoaded(this FrameworkElement frameworkElement, IScheduler scheduler = null)
		{
			if (scheduler == null)
			{
				return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
					h => frameworkElement.Loaded += h,
					h => frameworkElement.Loaded -= h)
								 .SelectUnit();
			}
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => frameworkElement.Loaded += h,
				h => frameworkElement.Loaded -= h,
				scheduler)
			                 .SelectUnit();
		}

		public static IObservable<Unit> ObserveUnloaded(this FrameworkElement frameworkElement, IScheduler scheduler = null)
		{
			if (scheduler == null)
			{
				return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => frameworkElement.Unloaded += h,
				h => frameworkElement.Unloaded -= h)
							 .SelectUnit();
			}
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => frameworkElement.Unloaded += h,
				h => frameworkElement.Unloaded -= h,
				scheduler)
							 .SelectUnit();
		}
	}
}
