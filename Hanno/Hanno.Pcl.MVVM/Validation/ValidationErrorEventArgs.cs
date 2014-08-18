using System;

namespace Hanno.Validation
{
    public class ValidationErrorEventArgs : EventArgs
    {
        public ValidationErrorEventArgs(string propertyName, IValidationError error, object value)
        {
            PropertyName = propertyName;
            Error = error;
	        Value = value;
        }

        public string PropertyName { get; set; }
        public IValidationError Error { get; set; }
	    public object Value { get; set; }
    }
}
