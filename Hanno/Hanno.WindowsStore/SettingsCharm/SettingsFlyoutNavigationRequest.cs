using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Hanno.Navigation;
using Hanno.ViewModels;

namespace Hanno.SettingsCharm
{
	/// <summary>
	/// Decorator of IRequestNavigation. Handle page navigations using a setting flyout
	/// </summary>
	public class SettingsFlyoutNavigationRequest : IRequestNavigation
	{
		private class SettingsFlyoutDefinition
		{
			public Type FlyoutType;
			public Type ViewModelType;
		}

		private readonly IRequestNavigation _inner;
		private readonly IViewModelFactory _viewModelFactory;
		private readonly IScheduler _dispatcher;
		private readonly Dictionary<string, SettingsFlyoutDefinition> _definitions;

		/// <summary>
		/// Create a new instance of SettingsFlyoutNavigationRequest
		/// </summary>
		/// <param name="inner">The inner request navigation.</param>
		/// <param name="viewModelFactory">The factory used to create the view models</param>
		/// <param name="dispatcher">Dispatcher scheduler</param>
		public SettingsFlyoutNavigationRequest(
			IRequestNavigation inner,
			IViewModelFactory viewModelFactory,
			IScheduler dispatcher)
		{
			if (inner == null) throw new ArgumentNullException("inner");
			if (viewModelFactory == null) throw new ArgumentNullException("viewModelFactory");
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			_inner = inner;
			_viewModelFactory = viewModelFactory;
			_dispatcher = dispatcher;
			_definitions = new Dictionary<string, SettingsFlyoutDefinition>();
		}

		/// <summary>
		/// Register a setting flyout
		/// </summary>
		/// <typeparam name="TFlyout">Flyout type. Must inherit from the SettingsFlyout class</typeparam>
		/// <typeparam name="TViewModel">View model type.</typeparam>
		/// <param name="pageName">Name of the page.</param>
		/// <returns></returns>
		public SettingsFlyoutNavigationRequest RegisterFlyout<TFlyout, TViewModel>(string pageName) where TFlyout : SettingsFlyout
		{
			_definitions.Add(pageName, new SettingsFlyoutDefinition()
			{
				FlyoutType = typeof(TFlyout),
				ViewModelType = typeof(TViewModel)
			});
			return this;
		}

		public async Task Navigate(CancellationToken ct, INavigationRequest request)
		{
			SettingsFlyoutDefinition definition;
			if (!_definitions.TryGetValue(request.PageName, out definition))
			{
				await _inner.Navigate(ct, request);
				return;
			}
			var viewModel = _viewModelFactory.ResolveViewModel(definition.ViewModelType);
			viewModel.Initialize(request);
			await _dispatcher.Run(() =>
			{
				var flyout = (SettingsFlyout)Activator.CreateInstance(definition.FlyoutType);
				flyout.DataContext = viewModel;
				flyout.Unloaded += (sender, args) => _viewModelFactory.ReleaseViewModel(viewModel);
				flyout.Show();
			});
			await viewModel.Load(ct);
		}
	}
}