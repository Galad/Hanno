using Hanno.Validation.Rules;

namespace Hanno.Validation
{
    public interface IRuleProvider
    {
        ValidationRule GetRule(string id);
    }

    public static class RuleProviderExtensions
    {
        public static ValidationRule<T> GetRule<T>(this IRuleProvider ruleProvider, string id)
        {
			return (ValidationRule<T>)ruleProvider.GetRule(id);
        }
    }
}