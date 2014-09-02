using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.Services;
using Hanno.ViewModels;

namespace TestUniversalApp
{
	public class SettingsFlyoutTestViewModel : ViewModelBase
	{
		private readonly IAsyncMessageDialog _asyncMessageDialog;

		public SettingsFlyoutTestViewModel(IViewModelServices services, IAsyncMessageDialog asyncMessageDialog) : base(services)
		{
			if (asyncMessageDialog == null) throw new ArgumentNullException("asyncMessageDialog");
			_asyncMessageDialog = asyncMessageDialog;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			CommandTest = GetCommandTest();
		}

		public ICommand CommandTest { get; private set; }

		private ICommand GetCommandTest()
		{
			return CommandBuilderProvider.Get("CommandTest")
			                             .Execute(async ct => await _asyncMessageDialog.Show(ct, "Hello", "From settings flyout"))
			                             .Error((token, exception) => Task.FromResult(true))
			                             .ToCommand();
		}
	}
}
