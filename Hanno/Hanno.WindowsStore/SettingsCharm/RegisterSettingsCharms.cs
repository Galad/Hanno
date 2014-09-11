using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Hanno.Globalization;
using Hanno.Navigation;
using Hanno.ViewModels;

namespace Hanno.SettingsCharm
{
	/// <summary>
	/// Register settings charms commands
	/// </summary>
	/// <typeparam name="TViewModel">Type of the settings charm view model</typeparam>
	public class RegisterSettingsCharms<TViewModel> : IObservable<Unit>, ISettingsCharmService where TViewModel : IViewModel
	{
		private readonly IScheduler _dispatcher;
		private readonly IResources _resources;
		private readonly List<CommandDefinition> _commandDefinitions;
		private readonly Lazy<TViewModel> _viewModel;
		private readonly string _settingsCharmPageName;

		/// <summary>
		/// Create a new instance of RegisterSettingsCharm
		/// </summary>
		/// <param name="dispatcher">Dispatcher scheduler</param>
		/// <param name="viewModelFactory">The factory used to create the view models</param>
		/// <param name="resources">The resource service</param>
		/// <param name="settingsCharmPageName">The name of the settings charm page. It will be exposed in the the NavigationRequest property of the view model.</param>
		public RegisterSettingsCharms(
			IScheduler dispatcher,
			IViewModelFactory viewModelFactory,
			IResources resources,
			string settingsCharmPageName)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (viewModelFactory == null) throw new ArgumentNullException("viewModelFactory");
			if (resources == null) throw new ArgumentNullException("resources");
			_dispatcher = dispatcher;
			_resources = resources;
			_settingsCharmPageName = settingsCharmPageName;
			_viewModel = new Lazy<TViewModel>(() =>
			{
				var viewModel = (TViewModel) viewModelFactory.ResolveViewModel(typeof (TViewModel));
				viewModel.Initialize(new NavigationRequest(_settingsCharmPageName, new Dictionary<string, string>()));
				return viewModel;
			});
			_commandDefinitions = new List<CommandDefinition>();
		}

		/// <summary>
		/// Observe the settings panel activation and adds command.
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public IDisposable Subscribe(IObserver<Unit> observer)
		{
			return Observable.Defer(() =>
			{
				var settingsPane = SettingsPane.GetForCurrentView();
				return Observable.FromEventPattern<TypedEventHandler<SettingsPane, SettingsPaneCommandsRequestedEventArgs>, SettingsPaneCommandsRequestedEventArgs>(
					h => settingsPane.CommandsRequested += h,
					h => settingsPane.CommandsRequested -= h,
					_dispatcher);
			})
			.Do(args =>
			{
				var settingsArgs = args.EventArgs;
				foreach (var commandDefinition in _commandDefinitions.Where(d => d.Command(_viewModel.Value).CanExecute(null)))
				{
					CommandDefinition definition = commandDefinition;
					settingsArgs.Request.ApplicationCommands.Add(new SettingsCommand(
						commandDefinition.Id,
						_resources.Get(commandDefinition.TitleKey),
						command => definition.Command(_viewModel.Value).Execute(null)));
				}
			})
			.SelectUnit()
							 .Subscribe(observer);
		}

		/// <summary>
		/// Add a command to the settings charm
		/// </summary>
		/// <param name="id">Id of the command. Must be unique</param>
		/// <param name="titleKey">Resource key of the command name</param>
		/// <param name="getCommand">Select the command to execute from the view model</param>
		/// <returns></returns>
		public RegisterSettingsCharms<TViewModel> AddCommand(string id, string titleKey, Func<TViewModel, ICommand> getCommand)
		{
			_commandDefinitions.Add(new CommandDefinition()
			{
				Id = id,
				TitleKey = titleKey,
				Command = getCommand
			});
			return this;
		}

		private class CommandDefinition
		{
			public string Id;
			public string TitleKey;
			public Func<TViewModel, ICommand> Command;
		}

		public void Show()
		{
			SettingsPane.Show();
		}
	}
}