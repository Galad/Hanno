using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace Hanno
{
	public static class NotifyCollectionChangedExtensions
	{
		public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged(this INotifyCollectionChanged collectionChanged)
		{
			return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
				h => collectionChanged.CollectionChanged += h,
				h => collectionChanged.CollectionChanged -= h)
			                 .Select(args => args.EventArgs);
		}
	}
}