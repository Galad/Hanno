using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Validation.Rules
{
    public abstract class ValidationRule : IValidationRule
    {
	    public Task<IValidationError> Validate(object value, CancellationToken cancellationToken)
        {
            return ValidateOverride(value, cancellationToken);
        }

        protected abstract Task<IValidationError> ValidateOverride(object value, CancellationToken cancellationToken);
    }

    public abstract class ValidationRule<T> : ValidationRule
    {
	    private Func<T, string> _messageFormatter = _ => string.Empty;

	    protected async override Task<IValidationError> ValidateOverride(object value, CancellationToken cancellationToken)
        {
            T tValue;
            try
            {
                tValue = (T)value;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException(string.Format("The rule cannot handle the type of the value passed in parameter {0}", value.GetType()), ex);
            }
            return await ValidateAsync(tValue, cancellationToken);
        }

	    protected virtual Task<IValidationError> ValidateAsync(T value, CancellationToken token)
	    {
		    return Task.Run(() => Validate(value), token);
	    } 

	    protected virtual IValidationError Validate(T value)
	    {
		    return NoError.Value;
	    }

	    public void SetMessageFormatter(Func<T, string> messageFormatter)
	    {
		    _messageFormatter = messageFormatter;
	    }

	    public string GetFormattedMessage(T value)
	    {
		    return _messageFormatter(value);
	    }
    }
}