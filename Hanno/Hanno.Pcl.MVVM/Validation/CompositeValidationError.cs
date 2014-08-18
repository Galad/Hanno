using System.Collections.Generic;
using System.Linq;

namespace Hanno.Validation
{
    public class CompositeValidationError : IValidationError
    {
        public CompositeValidationError(string message, IEnumerable<IValidationError> childErrors)
        {
            Message = message;
            ChildErrors = childErrors;
        }

        public string Message { get; private set; }
        public object ErrorData { get { return null; } }
        public IEnumerable<IValidationError> ChildErrors { get; private set; }
    }

    public class ValidationError : IValidationError
    {
        public ValidationError(string message, object errorData)
        {
            Message = message;
            ErrorData = errorData;
            ChildErrors = Enumerable.Empty<IValidationError>();
        }

        public string Message { get; private set; }
        public object ErrorData { get; private set; }
        public IEnumerable<IValidationError> ChildErrors { get; private set; }
    }
}