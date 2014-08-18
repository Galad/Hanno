namespace Hanno.Validation.Rules.Common
{
    public class StringMinimumLengthRule : ValidationRule<string>
    {
        public int MinimumLength { get; set; }

        protected override IValidationError Validate(string value)
        {
			return (value != null && value.Length >= MinimumLength) ? NoError.Value : new ValidationError(GetFormattedMessage(value), MinimumLength);
        }
    }
}