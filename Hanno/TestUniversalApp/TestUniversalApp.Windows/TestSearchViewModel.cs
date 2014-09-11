using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.Extensions;
using Hanno.Services;
using Hanno.ViewModels;

namespace TestUniversalApp
{
	public class TestSearchViewModel : ViewModelBase
	{
		private readonly IAsyncMessageDialog _messageDialog;

		public TestSearchViewModel(IViewModelServices services, IAsyncMessageDialog messageDialog)
			: base(services)
		{
			if (messageDialog == null) throw new ArgumentNullException("messageDialog");
			_messageDialog = messageDialog;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			Result1SuggestionCtrlCommand = GetResult1SuggestionCtrlCommand();
			Result1SuggestionAltCommand = GetResult1SuggestionAltCommand();
			Result2SuggestionCtrlCommand = GetResult2SuggestionCtrlCommand();
			Result2SuggestionAltCommand = GetResult2SuggestionAltCommand();
			DefaultCommand = GetDefaultCommand();
		}

		public ICommand Result1SuggestionCtrlCommand { get; private set; }
		public ICommand Result1SuggestionAltCommand { get; private set; }
		public ICommand Result2SuggestionCtrlCommand { get; private set; }
		public ICommand Result2SuggestionAltCommand { get; private set; }
		public ICommand DefaultCommand { get; private set; }

		public ICommand GetResult1SuggestionCtrlCommand()
		{
			return this.CommandBuilder("Result1SuggestionCtrlCommand")
					   .Execute<Guid>(async (id, ct) => await _messageDialog.Show(ct, "Result1SuggestionCtrlCommand", id.ToString()))
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		public ICommand GetResult1SuggestionAltCommand()
		{
			return this.CommandBuilder("Result1SuggestionAltCommand")
					   .Execute<Guid>(async (id, ct) => await _messageDialog.Show(ct, "Result1SuggestionAltCommand", id.ToString()))
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		public ICommand GetResult2SuggestionCtrlCommand()
		{
			return this.CommandBuilder("Result2SuggestionCtrlCommand")
					   .Execute<Guid>(async (id, ct) => await _messageDialog.Show(ct, "Result2SuggestionCtrlCommand", id.ToString()))
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		public ICommand GetResult2SuggestionAltCommand()
		{
			return this.CommandBuilder("Result2SuggestionAltCommand")
					   .Execute<Guid>(async (id, ct) => await _messageDialog.Show(ct, "Result2SuggestionAltCommand", id.ToString()))
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		public ICommand GetDefaultCommand()
		{
			return this.CommandBuilder("DefaultCommand")
			           .Execute<string>(async (tag, ct) => await _messageDialog.Show(ct, "DefaultCommand", tag))
			           .Error((token, exception) => Task.FromResult(true))
			           .ToCommand();
		}
	}
}