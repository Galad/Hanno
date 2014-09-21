using System;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Hanno.Xaml.Controls
{
	[ContentProperty(Name = "Commands")]
	public class ResultSuggestionCommands : FrameworkElement, ICommand
	{
		public ResultSuggestionCommands()
		{
			Commands = new DependencyObjectCollection();
			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
		{
			foreach (var command in Commands.OfType<FrameworkElement>())
			{
				command.DataContext = DataContext;
			}
		}

		public bool CanExecute(object parameter)
		{
			var args = (SearchBoxResultSuggestionChosenEventArgs)parameter;
			var command = GetCommand(args);
			if (command == null && DefaultCommand == null)
			{
				return false;
			}
			if (command == null)
			{
				return DefaultCommand.CanExecute(args.Tag);
			}
			return command.Command.CanExecute(command.CommandSelector.SelectTag(args.Tag));
		}

		public void Execute(object parameter)
		{
			var args = (SearchBoxResultSuggestionChosenEventArgs)parameter;
			var command = GetCommand(args);
			if (command == null && DefaultCommand == null)
			{
				return;
			}
			if (command == null)
			{
				DefaultCommand.Execute(args.Tag);
			}
			else
			{
				command.Command.Execute(command.CommandSelector.SelectTag(args.Tag));
			}
		}

		private ResultSuggestionCommand GetCommand(SearchBoxResultSuggestionChosenEventArgs parameter)
		{
			return Commands
				.Cast<ResultSuggestionCommand>()
				.FirstOrDefault(c => c.Command != null &&
									 c.CommandSelector != null &&
									 c.KeyModifier == (VirtualKeyModifiers2)parameter.KeyModifiers &&
									 c.CommandSelector.CanHandleTag(parameter.Tag));
		}

		public event EventHandler CanExecuteChanged;

		public ICommand DefaultCommand
		{
			get { return (ICommand)GetValue(DefaultCommandProperty); }
			set { SetValue(DefaultCommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DefaultCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DefaultCommandProperty =
			DependencyProperty.Register("DefaultCommand", typeof(ICommand), typeof(ResultSuggestionCommands), new PropertyMetadata(null));

		public DependencyObjectCollection Commands { get; private set; }
	}

	// Summary:
	//     Specifies the virtual key used to modify another keypress.
}