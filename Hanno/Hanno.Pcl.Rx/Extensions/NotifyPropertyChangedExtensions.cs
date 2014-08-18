using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using Hanno;
using Hanno.Extensions;

namespace System.ComponentModel
{
    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<T> ObserveProperty<T>(this INotifyPropertyChanged entity, Expression<Func<T>> property)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                 o => entity.PropertyChanged += o,
                                 o => entity.PropertyChanged -=o)
                             .Where(c => property.GetPropertyName() == c.EventArgs.PropertyName)
                             .Select(c => property.Compile()());
        }

        public static IObservable<Unit> ObserveProperties(this INotifyPropertyChanged entity, params Expression<Func<object>>[] propertiesName)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                 o => entity.PropertyChanged += o,
                                 o => entity.PropertyChanged -= o)
                             .Where(c => propertiesName.Any(p => p.GetPropertyName() == c.EventArgs.PropertyName))
                             .Select(c => Unit.Default);
        }
    }
}
