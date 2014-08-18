using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Navigation;
using Hanno.Validation;

namespace Hanno.ViewModels
{
	public interface IViewModelServices
	{
		IRuleProvider RuleProvider { get; }
		IValidator Validator { get; }
		ISchedulers Schedulers { get; }
		INavigationService NavigationService { get; }
		IRequestNavigation RequestNavigation { get; }
		IAsyncCommandBus CommandBus { get; }
		IAsyncQueryBus QueryBus { get; }
		ICommandEvents CommandEvents { get; }
		ICommandStateEvents CommandStateEvents { get; }
		IQueryStateEvents QueryStateEvents { get; }
	}

	public class ViewModelServices : IViewModelServices
	{
		public ViewModelServices(
			IRuleProvider ruleProvider,
			IObservableRegistrationService observableRegistration,
			IValidator validator,
			ISchedulers schedulers,
			INavigationService navigationService,
			IRequestNavigation requestNavigation,
			IAsyncCommandBus commandBus,
			IAsyncQueryBus queryBus,
			ICommandEvents commandEvents,
			ICommandStateEvents commandStateEvents,
			IQueryStateEvents queryStateEvents)
		{
			if (ruleProvider == null) throw new ArgumentNullException("ruleProvider");
			if (observableRegistration == null) throw new ArgumentNullException("observableRegistration");
			if (validator == null) throw new ArgumentNullException("validator");
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			if (navigationService == null) throw new ArgumentNullException("navigationService");
			if (requestNavigation == null) throw new ArgumentNullException("requestNavigation");
			if (commandBus == null) throw new ArgumentNullException("commandBus");
			if (queryBus == null) throw new ArgumentNullException("queryBus");
			if (commandEvents == null) throw new ArgumentNullException("commandEvents");
			if (commandStateEvents == null) throw new ArgumentNullException("commandStateEvents");
			if (queryStateEvents == null) throw new ArgumentNullException("queryStateEvents");
			Schedulers = schedulers;
			Validator = validator;
			ObservableRegistration = observableRegistration;
			RuleProvider = ruleProvider;
			CommandStateEvents = commandStateEvents;
			QueryStateEvents = queryStateEvents;
			CommandEvents = commandEvents;
			QueryBus = queryBus;
			CommandBus = commandBus;
			RequestNavigation = requestNavigation;
			NavigationService = navigationService;
		}

		public IRuleProvider RuleProvider { get; private set; }
		public IValidator Validator { get; private set; }
		public IObservableRegistrationService ObservableRegistration { get; private set; }
		public ISchedulers Schedulers { get; private set; }
		public INavigationService NavigationService { get; private set; }
		public IRequestNavigation RequestNavigation { get; private set; }
		public IAsyncCommandBus CommandBus { get; private set; }
		public IAsyncQueryBus QueryBus { get; private set; }
		public ICommandEvents CommandEvents { get; private set; }
		public ICommandStateEvents CommandStateEvents { get; private set; }
		public IQueryStateEvents QueryStateEvents { get; private set; }
	}
}
