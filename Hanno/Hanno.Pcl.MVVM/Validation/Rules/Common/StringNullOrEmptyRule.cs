namespace Hanno.Validation.Rules.Common
{
    public class StringNullOrEmptyRule : ValidationRule<string>
    {
        protected override IValidationError Validate(string value)
        {
            return string.IsNullOrEmpty(value) ? new ValidationError(GetFormattedMessage(value), null) : NoError.Value;
        }
    }
}