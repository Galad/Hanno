using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Extensions;
using Hanno.Validation;
using Hanno.Validation.Rules;
using Hanno.ViewModels;

namespace Hanno.MVVM
{
	public static class NotifyValidationErrorExtensions
	{
		public static IObservable<ValidationErrorEventArgs> ObserveValidationErrors(this INotifyValidationError validable, string property)
		{
			if (validable == null) throw new ArgumentNullException("validable");
			if (property == null) throw new ArgumentNullException("property");
			return Observable.FromEventPattern<EventHandler<ValidationErrorEventArgs>, ValidationErrorEventArgs>(
								o => validable.ErrorsChanged += o,
								o => validable.ErrorsChanged -= o)
							.Where(c => property == c.EventArgs.PropertyName)
							.Select(c => c.EventArgs);
		}

		public static IObservable<ValidationErrorEventArgs> ObserveValidationErrors<T>(this INotifyValidationError validable, Expression<Func<T>> property)
		{
			return validable.ObserveValidationErrors(property.GetPropertyName());
		}

		public static Task<bool> HasError<T>(this INotifyValidationError validable, Expression<Func<T>> property)
		{
			return validable.HasErrorAsync(property.GetPropertyName());
		}

		public static IObservable<Unit> WhenValidated<T, TValidable>(
			this TValidable validable,
			Expression<Func<TValidable, T>> property,
			Func<T, CancellationToken, Task> doTask) where TValidable : INotifyValidationError
		{
			return validable.WhenValidated(property.GetPropertyName(), doTask);
		}

		public static IObservable<Unit> WhenValidated<T, TValidable>(
			this TValidable validable,
			string property,
			Func<T, CancellationToken, Task> doTask) where TValidable : INotifyValidationError
		{
			return validable.ObserveValidationErrors(property)
							.Where(args => !args.Error.IsError())
							.Select((validationArgs) => (T)validationArgs.Value)
							.SelectMany(async (value, token) =>
							{
								await doTask(value, token);
								return Unit.Default;
							});
		}

		public static IObservable<TValue> ObserveValidPropertyChanged<TValue, TSource>(
			this TSource source,
			string property,
			Func<TValue> currentValue,
			IScheduler scheduler)
			where TSource : INotifyValidationError, IValidable
		{
			return Observable.Merge(
				source.ObserveValidationErrors(property)
					  .Select(args => new Tuple<IValidationError, TValue>(args.Error, (TValue)args.Value)),
				Observable.FromAsync(() => source.GetErrorAsync(property))
						  .Select(_ => new Tuple<IValidationError, TValue>(source.GetError(property), currentValue())))
							 .Where(args => !args.Item1.IsError())
							 .Select(args => args.Item2);
		}

		public static IObservable<TValue> ObserveValidPropertyChanged<TValue, TSource>(
			this TSource source,
			Expression<Func<TValue>> property,
			Func<TValue> currentValue,
			IScheduler scheduler)
			where TSource : INotifyValidationError, IValidable
		{
			return source.ObserveValidPropertyChanged<TValue, TSource>(property.GetPropertyName(), currentValue, scheduler);

		}

		public static Task<IValidationError> ValidateProperty<T, TValidable>(this TValidable validable, Expression<Func<T>> property, CancellationToken cancellationToken)
			where TValidable : IValidable, IBindable
		{
			return validable.Validator.ValidateProperty(validable.GetValue(property), property, cancellationToken);
		}

		public static ValidationRule<T> DefinePropertyRule<T, TViewModel>(this TViewModel viewModel, Expression<Func<T>> property, string ruleId)
			where TViewModel : IViewModel, IValidable
		{
			return viewModel.Validator.DefinePropertyRule(property, viewModel.Services.RuleProvider, ruleId);
		}

		public static ValidationRule<T> DefinePropertyRule<T, TViewModel>(this TViewModel viewModel, Expression<Func<T>> property, string ruleId, string message)
			where TViewModel : IViewModel, IValidable
		{
			return viewModel.Validator.DefinePropertyRule(property, viewModel.Services.RuleProvider, ruleId, message);
		}
	}
}