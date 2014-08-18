using Hanno.Validation;
using Hanno.Validation.Rules;
using Microsoft.Practices.Unity;

namespace TestUniversalApp.Composition
{
    public class UnityRuleProvider : IRuleProvider
    {
        private readonly IUnityContainer _container;

        public UnityRuleProvider(IUnityContainer container)
        {
            _container = container;
        }

        public ValidationRule GetRule(string id)
        {
            return (ValidationRule)_container.Resolve<IValidationRule>(id);
        }
    }
}