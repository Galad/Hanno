using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Validation.Rules
{
    public class CompositeValidationRule<T> : ValidationRule<T>
    {
        public readonly IValidationRule[] Rules;

        public CompositeValidationRule(params IValidationRule[] rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            Rules = rules;
        }
		
		protected async override Task<IValidationError> ValidateAsync(T value, CancellationToken cancellationToken)
		{
			var errorsTasks = Rules.Select(r => r.Validate(value, cancellationToken));
			var results = await Task.WhenAll(errorsTasks);
			var errors = results.Where(e => e.IsError()).ToArray();
		
			if (errors.Length == 0)
			{
				return NoError.Value;
			}
			var error = new CompositeValidationError(GetFormattedMessage(value), errors);
			return error;
		}
		
    }
}