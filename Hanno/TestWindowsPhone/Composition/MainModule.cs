using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castor.Composition;
using Hanno;
using Hanno.CqrsInfrastructure;
using Hanno.Diagnostics;
using Hanno.Phone.Rx.Concurrency;
using Hanno.Serialization;
using Hanno.Services;
using Hanno.Unity;
using Hanno.Validation;
using Hanno.Validation.Rules;
using Hanno.ViewModels;
using System.Reactive;
using Microsoft.Practices.Unity;

namespace TestWindowsPhone.Composition
{
	public class MainModule : IUnityModule
	{
		public void Register(IUnityContainer container)
		{
			container.RegisterType<ISchedulers>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => new WindowsPhoneSchedulers(DispatcherScheduler.Current)));
			container.RegisterViewModel(c => new MainViewModel());
			container.RegisterViewModel(c => new SecondViewModel());
			container.RegisterViewModel(c => new ThirdViewModel());
			container.RegisterType<ISerializer>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => new SafeStringSerializer(new XmlSerializer())));
			container.RegisterType<IDeserializer>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => new SafeStringDeserializer(new XmlSerializer())));
			container.RegisterType<IScheduler>("ThreadPoolScheduler", new ContainerControlledLifetimeManager(), new InjectionFactory(c => ThreadPoolScheduler.Instance));
			container.RegisterType<UnityCommandQueryHandlerFactory>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new UnityCommandQueryHandlerFactory(c)))
					 .RegisterType<IAsyncCommandHandlerFactory, UnityCommandQueryHandlerFactory>()
					 .RegisterType<IAsyncQueryCommandHandlerFactory, UnityCommandQueryHandlerFactory>();
			container.RegisterType<AsyncCommandQueryBus>(new ContainerControlledLifetimeManager());
			container.RegisterType<IAsyncCommandBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new DebuggerAsyncCommandBus(
						new PerformanceAsyncCommandBus(
							c.Resolve<AsyncCommandQueryBus>()))));
			container.RegisterType<IAsyncQueryBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new DebuggerAsyncQueryBus(
						new PerformanceAsyncQueryBus(
							c.Resolve<AsyncCommandQueryBus>()))));
			container.RegisterType<IObservableRegistrationService, ObservableRegistrationService>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory((a, b, c) => new EmptyObservableRegistrationService()));
			container.RegisterType<IRuleProvider, UnityRuleProvider>(new ContainerControlledLifetimeManager());
			container.RegisterType<IValidator, PropertyValidator>(new ContainerControlledLifetimeManager());
		}

		private class EmptyObservableRegistrationService : IObservableRegistrationService
		{
			public IDisposable RegisterObservable<TObservable>(IObservable<TObservable> observable, Func<Exception, string> errorKey)
			{
				return observable.Subscribe(_ => { }, e => { });
			}
		}
	}
}
