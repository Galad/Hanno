using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using Windows.System.Threading;
using Windows.UI.Xaml.Controls;
using Hanno;
using Hanno.Commands;
using Hanno.Commands.MvvmCommandVisitors;
using Hanno.Concurrency;
using Hanno.Diagnostics;
using Hanno.Globalization;
using Hanno.MVVM.Navigation;
using Hanno.Navigation;
using Hanno.Rx.Concurrency;
using Hanno.Services;
using Hanno.ViewModels;
using Microsoft.Practices.Unity;
using TestUniversalApp.Composition.Factories;

namespace TestUniversalApp.Composition
{
	public class WinRtModule : IUnityModule
	{
		private readonly Frame _frame;
		private readonly CoreDispatcherScheduler _dispatcher;

		public WinRtModule(Frame frame, CoreDispatcherScheduler dispatcher)
		{
			_frame = frame;
			_dispatcher = dispatcher;
		}

		public void Register(IUnityContainer container)
		{
			container.RegisterType<IViewModelServices, ViewModelServices>(new ContainerControlledLifetimeManager());
			container.RegisterType<ISchedulers>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new WinRtSchedulers(
						_dispatcher,
						c.Resolve<IPriorityScheduler>("PriorityDispatcher"),
						c.Resolve<IPriorityScheduler>("PriorityThreadPool"))));
			container.RegisterType<IPriorityScheduler, CoreDispatcherPriorityScheduler>(
				"PriorityThreadPool",
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new ThreadPoolPriorityScheduler()));
			container.RegisterType<IPriorityScheduler, CoreDispatcherPriorityScheduler>(
				"PriorityDispatcher",
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new ThreadPoolPriorityScheduler()));
			container.RegisterType<IScheduler>("DispatcherScheduler", new ContainerControlledLifetimeManager(), new InjectionFactory(c => _dispatcher));
			container.RegisterType<IScheduler>("ThreadPoolHigh", new ContainerControlledLifetimeManager(), new InjectionFactory(c => new ThreadPoolScheduler(WorkItemPriority.High)));
			container.RegisterType<Frame>(new ContainerControlledLifetimeManager(), new InjectionFactory(unityContainer => _frame));
			container.RegisterType<NavigationService>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new NavigationService(
					c.Resolve<Frame>(),
					c.Resolve<IPageDefinitionRegistry>(),
					c.Resolve<IViewModelFactory>(),
					c.Resolve<IScheduler>("DispatcherScheduler"),
					c.Resolve<IScheduler>("ThreadPoolHigh"))));
			container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IRequestNavigation, NavigationService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IPageDefinitionRegistry, PageDefinitionRegistry>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer => RegisterPages(new PageDefinitionRegistry())));
			container.RegisterType<UnityViewModelFactory>(new ContainerControlledLifetimeManager());
			container.RegisterType<IViewModelFactory, AddMvvmCommandVisitorsViewModelFactory>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer =>
					new MonitoringInstancesViewModelFactory(
						new AddMvvmCommandVisitorsViewModelFactory(
							container.Resolve<UnityViewModelFactory>(),
							container.ResolveAll<IMvvmCommandVisitor>()),
						TimeSpan.FromSeconds(5))));

			container.RegisterType<IResources>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer => ResourceLoaderService.Create()));
			container.RegisterType<IMvvmCommandVisitor, DisplayMessageWhenErrorOccursVisitor>("DisplayDefaultError", new ContainerControlledLifetimeManager());
			container.RegisterType<IAsyncMessageDialog, AsyncMessageDialog>(new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new AsyncMessageDialog(c.Resolve<IScheduler>("DispatcherScheduler"))));
		}

		private IPageDefinitionRegistry RegisterPages(IPageDefinitionRegistry registry)
		{
			return registry.RegisterViewModel<MainViewModel, MainPage>(ApplicationPages.Main)
						   .RegisterViewModel<SecondViewModel, SecondPage>(ApplicationPages.Second)
						   .RegisterViewModel<ThirdViewModel, ThirdPage>(ApplicationPages.Third);
		}
	}
}
