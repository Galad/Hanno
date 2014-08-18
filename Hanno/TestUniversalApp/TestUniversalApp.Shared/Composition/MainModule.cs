using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hanno.Commands;
using Hanno.Commands.MvvmCommandVisitors;
using Hanno.Diagnostics;
using Hanno.Unity;
using TestUniversalApp.Composition;
using Hanno;
using Hanno.CqrsInfrastructure;
using Hanno.Serialization;
using Hanno.Services;
using Hanno.Validation;
using Hanno.Validation.Rules;
using Hanno.ViewModels;
using System.Reactive;
using Microsoft.Practices.Unity;
using TestUniversalApp.Composition.Factories;

namespace TestUniversalApp.Composition
{
	public class MainModule : IUnityModule
	{
		public void Register(IUnityContainer container)
		{
			container.RegisterViewModel(c => new MainViewModel(c.Resolve<IEntityBuilder>()));
			container.RegisterViewModel(c => new SecondViewModel());
			container.RegisterViewModel(c => new ThirdViewModel());
			container.RegisterType<ISerializer>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => new SafeStringSerializer(new XmlSerializer())));
			container.RegisterType<IDeserializer>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => new SafeStringDeserializer(new XmlSerializer())));
			container.RegisterType<IScheduler>("ThreadPoolScheduler", new ContainerControlledLifetimeManager(), new InjectionFactory(c => ThreadPoolScheduler.Default));
			container.RegisterType<UnityCommandQueryHandlerFactory>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new UnityCommandQueryHandlerFactory(c)))
					 .RegisterType<IAsyncCommandHandlerFactory, UnityCommandQueryHandlerFactory>()
					 .RegisterType<IAsyncQueryCommandHandlerFactory, UnityCommandQueryHandlerFactory>();
			container.RegisterType<AsyncCommandQueryBus>(new ContainerControlledLifetimeManager());

			container.RegisterType<NotifyCommandStateBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new NotifyCommandStateBus(
						new PerformanceAsyncCommandBus(
							c.Resolve<AsyncCommandQueryBus>()))));

			container.RegisterType<NotifyQueryStateBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new NotifyQueryStateBus(
						new PerformanceAsyncQueryBus(
							c.Resolve<AsyncCommandQueryBus>()))));

#if DEBUG
			container.RegisterType<IAsyncCommandBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new DebuggerAsyncCommandBus(c.Resolve<NotifyCommandStateBus>())));
			container.RegisterType<IAsyncQueryBus>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c =>
					new DebuggerAsyncQueryBus(c.Resolve<NotifyQueryStateBus>())));
#else
			container.RegisterType<IAsyncCommandBus, NotifyCommandStateBus>();
			container.RegisterType<IAsyncQueryBus, NotifyQueryStateBus>();
#endif
			container.RegisterType<IEntityConverterFactory, UnityEntityConverterFactory>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new UnityEntityConverterFactory(c)));
			container.RegisterType<IEntityBuilder, EntityBuilder>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new EntityBuilder(c.Resolve<IEntityConverterFactory>())));
			container.RegisterTypes(new EntityConverterConvention(new[] {typeof (App).GetTypeInfo().Assembly}));

			container.RegisterType<IObservableRegistrationService, ObservableRegistrationService>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory((a, b, c) => new EmptyObservableRegistrationService()));
			container.RegisterType<IRuleProvider, UnityRuleProvider>(new ContainerControlledLifetimeManager());
			container.RegisterType<IValidator, PropertyValidator>(new ContainerControlledLifetimeManager());
			container.RegisterType<IMvvmCommandVisitor, DisplayMessageWhenErrorOccursVisitor>("sdff", new ContainerControlledLifetimeManager());
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
