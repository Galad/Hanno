using System;
using System.Threading.Tasks;

namespace Hanno.Validation
{
    public interface INotifyValidationError
    {
        event EventHandler<ValidationErrorEventArgs> ErrorsChanged;
        event EventHandler<ValidationErrorEventArgs> BeginValidation;
        IValidationError GetError(string propertyName);
		Task<IValidationError> GetErrorAsync(string propertyName);
	    Task<bool> HasErrorsAsync();
		Task<bool> HasErrorAsync(string propertyName);
    }
}