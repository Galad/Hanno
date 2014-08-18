using System;

namespace Hanno.Validation.Rules.Common
{
    public class GreaterThanRule<T> : ValidationRule<T> where T : IComparable<T>
    {
        public T GreaterThanValue { get; set; }

        protected override IValidationError Validate(T value)
        {
			return value.CompareTo(GreaterThanValue) == 1 ? NoError.Value : new ValidationError(GetFormattedMessage(value), GreaterThanValue);
        }
    }
}