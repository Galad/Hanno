using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Validation
{
    public interface IValidationRule
    {
	    /// <summary>
	    /// Validate the value with the specified rule
	    /// </summary>
	    /// <param name="value">The value to validate</param>
	    /// <param name="cancellationToken"></param>
	    /// <returns>Return an <see cref="IValidationError"/> if the the value is not valid according to the rule. If the value is valid, returns null.</returns>
	    Task<IValidationError> Validate(object value, CancellationToken cancellationToken);
    }
}