using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Hanno.Extensions;

namespace Hanno.ViewModels
{
    [System.Runtime.Serialization.DataContract]
    public class Bindable : INotifyPropertyChanged, IBindable
    {
        private readonly ISchedulers _schedulers;
        private readonly object locker = new object();
        private readonly IDictionary<string, object> repository = new Dictionary<string, object>();

        public Bindable(ISchedulers schedulers)
        {
            if (schedulers == null) throw new ArgumentNullException("schedulers");
            _schedulers = schedulers;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public virtual T GetValue<T>(Expression<Func<T>> key, Func<T> factory = null)
        {
            if (factory == null)
            {
                factory = () => default(T);
            }
            return GetValue(factory, key.GetPropertyName());
        }

        [System.Diagnostics.DebuggerStepThrough]
        public virtual T GetValue<T>(Func<T> factory = null, [CallerMemberName] string key = null)
        {
            lock (locker)
            {
                // if the value is not already inside the dictionary
                if (repository.ContainsKey(key) == false)
                {
                    repository[key] = (factory ?? (() => default(T)))();

                    NotifyPropertyChanged(key);
                }

                return (T)repository[key];
            }
        }

        /// <summary>
        /// Invalidates properties (clear the data from the repository and raise the PropertyChanged event)
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        protected virtual void Invalidate(params Expression<Func<object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                Invalidate(expression.GetPropertyName());
            }
        }

        /// <summary>
        /// Invalidates a property (clear the data from the repository and raise the PropertyChanged event)
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public virtual void Invalidate(string property)
        {
            if (repository.ContainsKey(property)) repository.Remove(property);

            NotifyPropertyChanged(property);
        }

        //[System.Diagnostics.DebuggerStepThrough]
        public virtual bool SetValue<T>(T value, [CallerMemberName] string key = null)
        {
            lock (locker)
            {
                if (EqualityComparer<T>.Default.Equals(value, GetValue(() => default(T), key)) == false)
                {
                    repository[key] = value;
                    NotifyPropertyChanged(key);
                    return true;
                }
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public bool SetValue<T>(T value, [CallerMemberName] string key = null, params Expression<Func<object>>[] expressions)
        {
            if (EqualityComparer<T>.Default.Equals(value, GetValue(() => default(T), key)) == false)
            {
                var hasChanged = SetValue(value, key);
                if (hasChanged)
                {
                    NotifyPropertyChanged(expressions);
                    return true;
                }
            }
            return false;
        }

        public void NotifyPropertyChanged(params Expression<Func<object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                NotifyPropertyChanged(expression.GetPropertyName());
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                _schedulers.SafeDispatch(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

	public interface IBindable
	{
		[System.Diagnostics.DebuggerStepThrough]
		T GetValue<T>(Func<T> factory = null, [CallerMemberName] string key = null);

		/// <summary>
		/// Invalidates a property (clear the data from the repository and raise the PropertyChanged event)
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		void Invalidate(string property);

		bool SetValue<T>(T value, [CallerMemberName] string key = null);
		void NotifyPropertyChanged([CallerMemberName] string propertyName = null);

		[System.Diagnostics.DebuggerStepThrough]
		T GetValue<T>(Expression<Func<T>> key, Func<T> factory = null);
	}
}