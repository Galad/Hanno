using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using Hanno.ViewModels;

namespace Hanno.Validation
{
	internal class RuleDefinerInterceptor : IValidator
	{
		private readonly Action<string> _interceptionAction;
		private readonly IValidator _innerDefiner;

		public RuleDefinerInterceptor(Action<string> interceptionAction, IValidator innerDefiner)
		{
			_interceptionAction = interceptionAction;
			_innerDefiner = innerDefiner;
		}

		public Task<IValidationError> Validate(object value, string key, CancellationToken cancellationToken)
		{
			return _innerDefiner.Validate(value, key, cancellationToken);
		}

		public void DefineRule(string key, IValidationRule rule)
		{
			_interceptionAction(key);
			_innerDefiner.DefineRule(key, rule);
		}

		public bool ContainsRule(string key)
		{
			return _innerDefiner.ContainsRule(key);
		}
	}

	public class Validable : Bindable, INotifyValidationError, IValidable
	{
		private readonly ISchedulers _scheduler;
		private readonly IDictionary<string, ValidationStatus> _errors = new Dictionary<string, ValidationStatus>();
		
		public IValidator Validator { get; private set; }

		#region Validation

		public Validable(IValidator validator, ISchedulers scheduler)
			: base(scheduler)
		{
			_scheduler = scheduler;
			Validator = CreateRuleDefiner(validator);
		}

		public Validable(ISchedulers scheduler)
			: base(scheduler)
		{
			_scheduler = scheduler;
			var v = new PropertyValidator();
			Validator = CreateRuleDefiner(v);
		}

		private IValidator CreateRuleDefiner(IValidator validator)
		{
			//force the creation of a status entry
			return new RuleDefinerInterceptor(key => GetErrorStatus(key), validator);
		}

		#endregion

		private class ValidationStatus
		{
			public IDisposable ValidatingDisposable
			{
				get { return _validatingDisposable; }
				set { _validatingDisposable = value; }
			}

			public bool Validating;
			public IValidationError ValidationError;
			public readonly ISubject<Unit> ValidationErrorSubject;
			private IDisposable _validatingDisposable;

			public ValidationStatus(IScheduler scheduler = null)
			{
				ValidationErrorSubject = scheduler != null ? new ReplaySubject<Unit>(1, scheduler) : new ReplaySubject<Unit>(1);
				ValidationErrorSubject.OnNext(Unit.Default);
			}
		}

		public override bool SetValue<T>(T value, [CallerMemberName] string key = null)
		{
			var hasChanged = base.SetValue(value, key);
#pragma warning disable 4014
			//we want to execute the validation asynchronously
			ValidateValueAsync(value, key, hasChanged, new CancellationTokenSource());
#pragma warning restore 4014
			return hasChanged;
		}

		protected virtual async Task<bool> SetValueAsync<T>(T value, string key, CancellationTokenSource cancellationTokenSource)
		{
			var hasChanged = base.SetValue(value, key);
			await ValidateValueAsync(value, key, hasChanged, cancellationTokenSource);
			return hasChanged;
		}

		public override T GetValue<T>(Func<T> factory = null, [CallerMemberName] string key = null)
		{
			return base.GetValue(() =>
			{
				if (factory == null)
				{
					factory = () => default(T);
				}
				var value = factory();
#pragma warning disable 4014
				//we want to execute the validation asynchronously
				ValidateValueAsync(value, key, true, new CancellationTokenSource());
#pragma warning restore 4014
				return value;
			}, key);
		}

		private async Task ValidateValueAsync<T>(T value, string key, bool hasChanged, CancellationTokenSource cancellationTokenSource)
		{
			if (!Validator.ContainsRule(key))
			{
				Validator.DefineRule(key, new NoErrorValidationRule());
			}

			if (hasChanged)
			{
				//validation
				NotifyBeginValidation(key, value);
				var status = GetErrorStatus(key);
				if (status.ValidatingDisposable != null)
				{
					status.ValidatingDisposable.Dispose();
				} // cancel a previous validation
				status.Validating = true;
				status.ValidationErrorSubject.OnNext(Unit.Default); // notify the beginning of the validation
				status.ValidatingDisposable = Disposable.Create(() =>
				{
					cancellationTokenSource.Cancel();
					cancellationTokenSource.Dispose();
				});
				var error = await Validator.Validate(value, key, cancellationTokenSource.Token);
				var existingError = status.ValidationError;
				status.ValidationError = error;
				if (error != existingError || status.Validating)
				{
					NotifyError(key, error, value);
					status.Validating = false;
					status.ValidationErrorSubject.OnNext(Unit.Default);
				}
			}
		}


		public event EventHandler<ValidationErrorEventArgs> ErrorsChanged;
		public event EventHandler<ValidationErrorEventArgs> BeginValidation;

		private ValidationStatus GetErrorStatus(string propertyName)
		{
			if (!_errors.ContainsKey(propertyName))
			{
				_errors[propertyName] = new ValidationStatus(_scheduler.CurrentThread)
				{
					ValidatingDisposable = null,
					ValidationError = NoError.Value
				};
			}
			return _errors[propertyName];
		}

		public IValidationError GetError(string propertyName)
		{
			return GetErrorStatus(propertyName).ValidationError;
		}

		public async Task<IValidationError> GetErrorAsync(string propertyName)
		{
			var status = GetErrorStatus(propertyName);
			return await status.ValidationErrorSubject
							   .Where(_ => !status.Validating)
							   .Select(_ => status.ValidationError)
							   .Take(1);
		}

		public async Task<bool> HasErrorsAsync()
		{
			return await _errors.Values.Select(e => e.ValidationErrorSubject
													.Select(_ => e)
													.StartWith(ImmediateScheduler.Instance, e)
													.Where(status => !status.Validating))
							   .CombineLatest()
							   .Where(status => status.All(s => !s.Validating))
							   .Select(status => status.Any(e => e.ValidationError.IsError()))
							   .Take(1);
		}

		public async Task<bool> HasErrorAsync(string propertyName)
		{
			ValidationStatus status = GetErrorStatus(propertyName);
			return await status.ValidationErrorSubject
							   .Select(_ => status)
							   .StartWith(_scheduler.Immediate, status)
							   .Where(s => !s.Validating)
							   .Select(s => s.ValidationError.IsError())
							   .Take(1);
		}

		public void NotifyError(string propertyName, IValidationError error, object value)
		{
			if (ErrorsChanged != null)
			{
				_scheduler.SafeDispatch(() => ErrorsChanged(this, new ValidationErrorEventArgs(propertyName, error, value)));
			}
		}
		
		private void NotifyBeginValidation(string propertyName, object value)
		{
			if (BeginValidation != null)
			{
				_scheduler.SafeDispatch(() => BeginValidation(this, new ValidationErrorEventArgs(propertyName, null, value)));
			}
		}
	}

	public class NoErrorValidationRule : IValidationRule
	{
		public Task<IValidationError> Validate(object value, CancellationToken cancellationToken)
		{
			return Task.FromResult(NoError.Value);
		}
	}
}