using Hanno.Validation.Rules;

namespace Hanno.Validation
{
	public class NoRuleProvider : IRuleProvider
	{
		public ValidationRule GetRule(string id)
		{
			return null;
		}
	}
}