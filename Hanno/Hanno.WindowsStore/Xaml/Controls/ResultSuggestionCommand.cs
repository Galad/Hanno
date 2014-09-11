using System.Windows.Input;
using Windows.UI.Xaml;

namespace Hanno.Xaml.Controls
{
	public class ResultSuggestionCommand : FrameworkElement
	{
		public VirtualKeyModifiers2 KeyModifier
		{
			get { return (VirtualKeyModifiers2)GetValue(KeyModifierProperty); }
			set { SetValue(KeyModifierProperty, value); }
		}

		// Using a DependencyProperty as the backing store for KeyModifier.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty KeyModifierProperty =
			DependencyProperty.Register("KeyModifier", typeof(VirtualKeyModifiers2), typeof(ResultSuggestionCommand), new PropertyMetadata(VirtualKeyModifiers2.None));

		public ISearchResultCommandSelector CommandSelector
		{
			get { return (ISearchResultCommandSelector)GetValue(CommandSelectorProperty); }
			set { SetValue(CommandSelectorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CommandSelector.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandSelectorProperty =
			DependencyProperty.Register("CommandSelector", typeof(ISearchResultCommandSelector), typeof(ResultSuggestionCommand), new PropertyMetadata(null));



		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(ResultSuggestionCommand), new PropertyMetadata(null));


	}
}