using Microsoft.Practices.Unity;

namespace TestUniversalApp.Composition
{
    public interface IUnityModule
    {
        void Register(IUnityContainer container);
    }
}