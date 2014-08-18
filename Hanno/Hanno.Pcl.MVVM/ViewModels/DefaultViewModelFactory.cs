using System;

namespace Hanno.ViewModels
{
    public class DefaultViewModelFactory : IViewModelFactory
    {
        public IViewModel ResolveViewModel(object request)
        {
            var type = request as Type;
            if (type == null)
            {
                return null;
            }
            return GetViewModelInstance(type);
        }

        public void ReleaseViewModel(IViewModel viewModel)
        {
            if (viewModel != null)
            {
                ReleaseViewModelInstance(viewModel);
            }
        }

        protected virtual IViewModel GetViewModelInstance(Type type)
        {
            return (IViewModel)Activator.CreateInstance(type);
        }

        protected virtual void ReleaseViewModelInstance(IViewModel viewModel)
        {
            var disposable = viewModel as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}