using System;
using System.Reactive;
using System.Reactive.Linq;
using Hanno;

namespace System.Windows
{
    public static class FrameworkElementExtensions
    {
        public static IObservable<Unit> ObserveLoaded(this FrameworkElement element)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                                 o => element.Loaded += o,
                                 o => element.Loaded -= o)
                             .SelectUnit();
        }

        public static IObservable<Unit> ObserveUnloaded(this FrameworkElement element)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                                 o => element.Unloaded += o,
                                 o => element.Unloaded -= o)
                             .SelectUnit();
        }

        public static IObservable<Unit> ObserveSizeChanged(this FrameworkElement element)
        {
            return Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(
                                 o => element.SizeChanged += o,
                                 o => element.SizeChanged -= o)
                             .SelectUnit();
        }

        public static IObservable<Unit> ObserveLayoutUpdated(this FrameworkElement element)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(
                                 o => element.LayoutUpdated += o,
                                 o => element.LayoutUpdated -= o)
                             .SelectUnit();
        }
    }
}
