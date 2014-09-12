using System;
using System.Collections.Generic;
using System.Reactive;
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
#if WINDOWS_APP
using Hanno.SettingsCharm;
#endif

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
#if WINDOWS_APP
			container.RegisterType<ISettingsCharmService, RegisterSettingsCharms<SettingsCharmViewModel>>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					RegisterSettingsCharmCommands(new RegisterSettingsCharms<SettingsCharmViewModel>(
						c.Resolve<IScheduler>("DispatcherScheduler"),
						c.Resolve<IViewModelFactory>(),
						c.Resolve<IResources>(),
						ApplicationPages.SettingsCharm))));
			container.RegisterType<IObservable<Unit>>(
				"RegisterSettingsCharm",
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => c.Resolve<RegisterSettingsCharms<SettingsCharmViewModel>>()));
			container.RegisterType<IRequestNavigation>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					RegisterSettingsFlyouts(new SettingsFlyoutNavigationRequest(
						c.Resolve<NavigationService>(),
						c.Resolve<IViewModelFactory>(),
						c.Resolve<IScheduler>("DispatcherScheduler")))));
#else
			container.RegisterType<IRequestNavigation, NavigationService>(new ContainerControlledLifetimeManager());
			
#endif

			container.RegisterType<IPageDefinitionRegistry, PageDefinitionRegistry>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer => RegisterPages(new PageDefinitionRegistry())));
			container.RegisterType<UnityViewModelFactory>(new ContainerControlledLifetimeManager());
			container.RegisterType<IViewModelFactory, AddMvvmVisitorsViewModelFactory>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer =>
					new MonitoringInstancesViewModelFactory(
						new AddMvvmVisitorsViewModelFactory(
							container.Resolve<UnityViewModelFactory>(),
							container.ResolveAll<IMvvmCommandVisitor>(),
							container.ResolveAll<IObservableViewModelVisitor>()),
						TimeSpan.FromSeconds(5))));

			container.RegisterType<IResources>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(unityContainer => ResourceLoaderService.Create()));
			container.RegisterType<IMvvmCommandVisitor, DisplayMessageWhenErrorOccursVisitor>("DisplayDefaultError", new ContainerControlledLifetimeManager());
			container.RegisterType<IAsyncMessageDialog, AsyncMessageDialog>(new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new AsyncMessageDialog(c.Resolve<IScheduler>("DispatcherScheduler"))));
			container.RegisterType<IObservableViewModelVisitor, NullReferenceEmptyPredicateVisitor>("NullReferenceEmptyPredicate", new ContainerControlledLifetimeManager());
			container.RegisterType<IObservableViewModelVisitor, EnumerableEmptyPredicateVisitor>("EnumerableEmptyPredicate", new ContainerControlledLifetimeManager());
			container.RegisterType<IViewModelServices, ViewModelServices>(new ContainerControlledLifetimeManager());
		}

#if WINDOWS_APP
		private RegisterSettingsCharms<SettingsCharmViewModel> RegisterSettingsCharmCommands(RegisterSettingsCharms<SettingsCharmViewModel> settingsCharms)
		{
			return settingsCharms.AddCommand("Command1", "Command1Name", viewModel => viewModel.SettingCommand1)
								 .AddCommand("Command2", "Command2Name", viewModel => viewModel.SettingCommand2);
		}

		private SettingsFlyoutNavigationRequest RegisterSettingsFlyouts(SettingsFlyoutNavigationRequest settings)
		{
			return settings.RegisterFlyout<SettingsFlyoutTest, SettingsFlyoutTestViewModel>(ApplicationPages.SettingsFlyoutTest);
		} 
#endif

		private IPageDefinitionRegistry RegisterPages(IPageDefinitionRegistry registry)
		{
			return registry.RegisterViewModel<MainViewModel, MainPage>(ApplicationPages.Main)
			               .RegisterViewModel<SecondViewModel, SecondPage>(ApplicationPages.Second)
			               .RegisterViewModel<ThirdViewModel, ThirdPage>(ApplicationPages.Third)
			               .RegisterViewModel<TestCachePageViewModel, TestCachePage>(ApplicationPages.TestCache)
#if WINDOWS_APP
			               .RegisterViewModel<TestSearchViewModel, TestSearch>(ApplicationPages.TestSearch)
#endif
				;
		}
	}
}
