namespace Hanno.Validation.Rules.Common
{
    public class StringMaximumLengthRule : ValidationRule<string>
    {
        public int MaximumLength { get; set; }

        protected override IValidationError Validate(string value)
        {
			return (value == null || value.Length <= MaximumLength) ? NoError.Value : new ValidationError(GetFormattedMessage(value), MaximumLength);
        }
    }

    public static class StringMaximumLengthRuleExtensions
    {
        public static StringMaximumLengthRule Length(this StringMaximumLengthRule rule, int length)
        {
            rule.MaximumLength = length;
            return rule;
        }
    }
}