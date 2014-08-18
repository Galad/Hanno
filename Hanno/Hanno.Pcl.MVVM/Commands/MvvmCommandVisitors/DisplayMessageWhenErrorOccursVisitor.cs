using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.Globalization;
using Hanno.Services;

namespace Hanno.Commands.MvvmCommandVisitors
{
	public sealed class DisplayMessageWhenErrorOccursVisitor : IMvvmCommandVisitor
	{
		private readonly IAsyncMessageDialog _messageDialog;
		private readonly IResources _resources;
		public const string TitleResourceKeySuffix = "_Error_Title";
		public const string ContentResourceKeySuffix = "_Error_Content";

		public DisplayMessageWhenErrorOccursVisitor(
			IAsyncMessageDialog messageDialog,
			IResources resources)
		{
			if (messageDialog == null) throw new ArgumentNullException("messageDialog");
			if (resources == null) throw new ArgumentNullException("resources");
			_messageDialog = messageDialog;
			_resources = resources;
		}

		public void Visit(IAsyncMvvmCommand command)
		{
			var errorTitleKey = string.Concat(command.Name, TitleResourceKeySuffix);
			var errorContentKey = string.Concat(command.Name, ContentResourceKeySuffix);
			var errorTitle = _resources.Get(errorTitleKey);
			var errorContent = _resources.Get(errorContentKey);
			var isDefault = command.SetDefaultError(async (ct, ex) =>
			{
				await _messageDialog.Show(ct, errorTitle, errorContent);
			});
			if (isDefault && (string.IsNullOrEmpty(errorTitle) || string.IsNullOrEmpty(errorContent)))
			{
				var sb = new StringBuilder();
				sb.AppendLine(string.Concat("Resources missing for command ", command.Name));
				sb.AppendLine("Missing resources keys :");
				if (string.IsNullOrEmpty(errorTitle))
				{
					sb.AppendLine(errorTitleKey);
				}
				if (string.IsNullOrEmpty(errorContent))
				{
					sb.AppendLine(errorContentKey);
				}
				throw new InvalidOperationException(sb.ToString());
			}
		}

		public void Visit(ICommand command)
		{
		}

		public void Visit<TCommand, TObservable>(IAsyncMvvmCommand<TCommand, TObservable> command)
		{
		}
	}
}