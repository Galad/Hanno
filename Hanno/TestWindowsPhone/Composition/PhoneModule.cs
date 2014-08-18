using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Castor.Composition;
using Castor.Composition.Factories;
using Hanno.Navigation;
using Hanno.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Practices.Unity;

namespace TestWindowsPhone.Composition
{
	public class PhoneModule : IUnityModule
	{
		private readonly PhoneApplicationFrame _frame;
		private readonly IScheduler _dispatcher;

		public PhoneModule(PhoneApplicationFrame frame, IScheduler dispatcher)
		{
			if (frame == null) throw new ArgumentNullException("frame");
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			_frame = frame;
			_dispatcher = dispatcher;
		}

		public void Register(IUnityContainer container)
		{
			container.RegisterType<IViewModelServices, ViewModelServices>();
			container.RegisterType<IScheduler>("DispatcherScheduler", new ContainerControlledLifetimeManager(), new InjectionFactory(c => _dispatcher));
			container.RegisterType<PhoneApplicationFrame>(new ContainerControlledLifetimeManager(), new InjectionFactory(unityContainer => _frame));
			container.RegisterType<PhoneNavigationService>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new PhoneNavigationService(
					c.Resolve<PhoneApplicationFrame>(),
					c.Resolve<IPageDefinitionRegistry>(),
					c.Resolve<IViewModelFactory>(),
					c.Resolve<IScheduler>("DispatcherScheduler"))));
			container.RegisterType<RemoveFirstEntryRequestNavigation>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new RemoveFirstEntryRequestNavigation(
						c.Resolve<PhoneNavigationService>(),
						_dispatcher,
						new Uri("/EntryPoint.xaml", UriKind.Relative),
						c.Resolve<PhoneApplicationFrame>())));
			container.RegisterType<INavigationService, PhoneNavigationService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IRequestNavigation, RemoveFirstEntryRequestNavigation>(new ContainerControlledLifetimeManager());
			container.RegisterType<IPageDefinitionRegistry, PageDefinitionRegistry>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer => RegisterPages(new PageDefinitionRegistry())));
			container.RegisterType<IViewModelFactory, UnityViewModelFactory>(new ContainerControlledLifetimeManager());
		}

		private IPageDefinitionRegistry RegisterPages(IPageDefinitionRegistry registry)
		{
			return registry.RegisterViewModel<MainViewModel, MainPage>(ApplicationPages.Main, new Uri("/TestWindowsPhone;component/MainPage.xaml", UriKind.Relative))
						   .RegisterViewModel<SecondViewModel, SecondPage>(ApplicationPages.Second, new Uri("/TestWindowsPhone;component/SecondPage.xaml", UriKind.Relative))
						   .RegisterViewModel<ThirdViewModel, ThridPage>(ApplicationPages.Third, new Uri("/TestWindowsPhone;component/ThridPage.xaml", UriKind.Relative));
		}
	}
}
