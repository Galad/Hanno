using System;

namespace Hanno.Validation.Rules
{
    public static class ValidationRuleExtensions
    {
		public static ValidationRule<T>  SetMessage<T>(this ValidationRule<T> validationRule, string message)
        {
            if (validationRule == null)
            {
                return null;
            }
			validationRule.SetMessageFormatter(_ => message);
            return validationRule;
        }

		public static ValidationRule<T>  SetMessage<T>(this ValidationRule<T> validationRule, Func<T, string> message)
		{
			if (validationRule == null)
			{
				return null;
			}
			validationRule.SetMessageFormatter(message);
			return validationRule;
		}
    }
}