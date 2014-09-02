using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.Navigation;
using Hanno.Services;
using Hanno.ViewModels;

namespace TestUniversalApp
{
	public class SettingsCharmViewModel : ViewModelBase
	{
		private readonly IAsyncMessageDialog _messageDialog;

		public SettingsCharmViewModel(IAsyncMessageDialog messageDialog, IViewModelServices services)
			: base(services)
		{
			if (messageDialog == null) throw new ArgumentNullException("messageDialog");
			_messageDialog = messageDialog;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			SettingCommand1 = GetSettingCommand1();
			SettingCommand2 = GetSettingCommand2();
		}

		public ICommand SettingCommand1 { get; private set; }
		public ICommand SettingCommand2 { get; private set; }

		private ICommand GetSettingCommand1()
		{
			return CommandBuilderProvider.Get("SettingCommand1")
										 .Execute(async ct => await _messageDialog.Show(ct, "Hello", "From the settings charm bar"))
										 .Error((token, exception) => Task.FromResult(true))
										 .ToCommand();
		}

		private ICommand GetSettingCommand2()
		{
			return CommandBuilderProvider.Get("SettingCommand2")
										 .Execute(async ct => await Services.RequestNavigation.Navigate(ct, ApplicationPages.SettingsFlyoutTest))
										 .Error((token, exception) => Task.FromResult(true))
										 .ToCommand();
		}
	}
}
