using Hanno.ViewModels;
using Microsoft.Practices.Unity;

namespace Castor.Composition.Factories
{
    public class UnityViewModelFactory : DefaultViewModelFactory
    {
        private readonly IUnityContainer _container;

        public UnityViewModelFactory(IUnityContainer container)
        {
            _container = container;
        }

        protected override IViewModel GetViewModelInstance(System.Type type)
        {
            return (IViewModel) _container.Resolve(type);
        }
    }
}