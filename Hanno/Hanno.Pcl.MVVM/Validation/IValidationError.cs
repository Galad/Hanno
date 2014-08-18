using System.Collections.Generic;
using System.Linq;

namespace Hanno.Validation
{
    public interface IValidationError
    {
        string Message { get; }
        object ErrorData { get; }
        IEnumerable<IValidationError> ChildErrors { get; }
    }

    public class NoError : IValidationError
    {
        private static readonly NoError value;
        static NoError()
        {
            value = new NoError();
        }

        public static IValidationError Value { get { return value; } }

        private NoError() { }
        public string Message
        {
            get { return string.Empty; }
        }

        public object ErrorData
        {
            get { return null; }
        }

        public IEnumerable<IValidationError> ChildErrors
        {
            get { return Enumerable.Empty<IValidationError>(); }
        }
    }

    public static class ValidationErrorExtensions
    {
        public static bool IsError(this IValidationError validationError)
        {
            return validationError != NoError.Value;
        }
    }
}