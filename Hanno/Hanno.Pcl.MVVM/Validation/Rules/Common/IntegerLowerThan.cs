using System;

namespace Hanno.Validation.Rules.Common
{
    public class LowerThanRule<T> : ValidationRule<T> where T : IComparable<T>
    {
        public T LowerThanValue { get; set; }

        protected override IValidationError Validate(T value)
        {
			return value.CompareTo(LowerThanValue) == -1 ? NoError.Value : new ValidationError(GetFormattedMessage(value), LowerThanValue);
        }
    }
}