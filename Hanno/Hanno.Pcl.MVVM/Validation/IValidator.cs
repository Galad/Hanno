using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Extensions;
using Hanno.Validation.Rules;

namespace Hanno.Validation
{
    public interface IValidator
    {
	    /// <summary>
	    /// Validate the value with the specified rule
	    /// </summary>
	    /// <param name="value">The value to validate</param>
	    /// <param name="key">The name of the rule to apply. The rule must have been define with <see cref="IRuleDefiner.DefineRule"/>.</param>
	    /// <param name="cancellationToken"></param>
	    /// <returns>Return an <see cref="IValidationError"/> if the the value is not valid according to the rule. If the value is valid, returns null.</returns>
	    Task<IValidationError> Validate(object value, string key, CancellationToken cancellationToken);
		void DefineRule(string key, IValidationRule rule);
		bool ContainsRule(string key);
    }

    public static class ValidatorExtensions
    {
		public static ValidationRule<T> DefinePropertyRule<T>(this IValidator validator, Expression<Func<T>> property, IRuleProvider ruleProvider, string ruleId)
		{
			var rule = ruleProvider.GetRule<T>(ruleId);
			validator.DefineRule(property.GetPropertyName(), rule);
			return rule;
		}

		public static ValidationRule<T> DefinePropertyRule<T>(this IValidator validator, Expression<Func<T>> property, IRuleProvider ruleProvider, string ruleId, string message)
		{
			var rule = ruleProvider.GetRule<T>(ruleId)
			                       .SetMessage(message);
			validator.DefineRule(property.GetPropertyName(), rule);
			return rule;
		}
		
        public static Task<IValidationError> ValidateProperty<T>(this IValidator validator, object value, Expression<Func<T>> property, CancellationToken cancellationToken)
        {
            return validator.Validate(value, property.GetPropertyName(), cancellationToken);
        }
    }
}