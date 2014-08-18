using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.ViewModels;
using Microsoft.Practices.Unity;

namespace Microsoft.Practices.Unity
{
	public static class UnityContainerExtensions
	{
		public static void RegisterViewModel<TViewModel>(
			this IUnityContainer container,
			Func<IUnityContainer, TViewModel> factory,
			Func<IUnityContainer, IViewModelServices> viewModelServicesFactory = null)
			where TViewModel : ViewModelBase
		{
			if (viewModelServicesFactory == null)
			{
				viewModelServicesFactory = c => c.Resolve<IViewModelServices>();
			}
			container.RegisterType<TViewModel>(
				new InjectionFactory(c =>
				{
					var vm = factory(c);
					vm.Services = viewModelServicesFactory(c);
					return vm;
				}));
		}
	}
}
