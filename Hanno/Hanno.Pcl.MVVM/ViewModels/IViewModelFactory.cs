using System;

namespace Hanno.ViewModels
{
    public interface IViewModelFactory
    {
        IViewModel ResolveViewModel(object request);
        void ReleaseViewModel(IViewModel viewModel);
    }

    public static class ViewModelFactoryExtensions
    {
        public static IViewModel ResolveViewModel<T>(this IViewModelFactory factory) where T : IViewModel
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return factory.ResolveViewModel(typeof (T));
        }
    }
}