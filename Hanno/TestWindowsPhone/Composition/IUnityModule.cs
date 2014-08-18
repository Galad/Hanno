using Microsoft.Practices.Unity;

namespace Castor.Composition
{
    public interface IUnityModule
    {
        void Register(IUnityContainer container);
    }
}