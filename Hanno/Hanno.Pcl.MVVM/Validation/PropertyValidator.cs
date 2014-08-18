using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Validation
{
    public class PropertyValidator : IValidator
    {
        private readonly Dictionary<string, IValidationRule> _rules;

        public PropertyValidator()
        {
            _rules = new Dictionary<string, IValidationRule>();
        }

        public void DefineRule(string key, IValidationRule rule)
        {
            _rules[key] = rule;
        }

        public bool ContainsRule(string key)
        {
            return _rules.ContainsKey(key);
        }

        public Task<IValidationError> Validate(object value, string key, CancellationToken cancellationToken)
        {
            if (!_rules.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("The rule {0} has not been registered", key), key);
            }
            var rule = _rules[key];
			var result = rule.Validate(value, cancellationToken);
            return result;
        }
    }
}