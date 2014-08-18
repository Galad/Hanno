using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace Hanno.Controls
{
	public class AppBarIconItem : FrameworkElement
	{
		public ApplicationBarIconButton ApplicationBarIconButton { get; private set; }
		public int Index;
		private IDisposable _canExecuteDisposable;

		public AppBarIconItem()
		{
			ApplicationBarIconButton = new ApplicationBarIconButton();
			ApplicationBarIconButton.Click += _applicationBarIconButton_Click;
		}

		void _applicationBarIconButton_Click(object sender, EventArgs e)
		{
			if (Command != null && Command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
			}
		}

		public Uri IconUri
		{
			get { return (Uri)GetValue(IconUriProperty); }
			set { SetValue(IconUriProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IconUri.  This enables animation, styling, binding, etc...

		public static readonly DependencyProperty IconUriProperty =
			DependencyProperty.Register("IconUri", typeof(Uri), typeof(AppBarIconItem), new PropertyMetadata(null, OnUriChanged));

		private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AppBarIconItem)d).ApplicationBarIconButton.IconUri = (Uri)e.NewValue;
		}


		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(AppBarIconItem), new PropertyMetadata(null, OnTextChanged));

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AppBarIconItem)d).ApplicationBarIconButton.Text = (string)e.NewValue;
		}


		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(AppBarIconItem), new PropertyMetadata(null, OnCommandChanged));

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AppBarIconItem) d).UpdateCanExecute((ICommand) e.NewValue);
		}

		void UpdateCanExecute(ICommand command)
		{
			if (command == null)
			{
				SetCanExecuteDisposable(null);
				return;
			}
			command.CanExecuteChanged += command_CanExecuteChanged;
			SetCanExecuteDisposable(new ActionDisposable(() => command.CanExecuteChanged -= command_CanExecuteChanged));
		}

		void command_CanExecuteChanged(object sender, EventArgs e)
		{
			ApplicationBarIconButton.IsEnabled = ((ICommand) sender).CanExecute(CommandParameter);
		}


		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(AppBarIconItem), new PropertyMetadata(null));
		

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsVisible.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(AppBarIconItem), new PropertyMetadata(true, OnVisibilityChanged));

		private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = (AppBarIconItem)d;
			if (element.VisibilityChanged != null)
			{
				element.VisibilityChanged(element, EventArgs.Empty);
			}
		}

		public event EventHandler VisibilityChanged;

		private void SetCanExecuteDisposable(IDisposable disposable)
		{
			if (_canExecuteDisposable != null)
			{
				_canExecuteDisposable.Dispose();
			}
			_canExecuteDisposable = disposable;
		}

		private class ActionDisposable : IDisposable
		{
			private readonly Action _action;

			public ActionDisposable(Action action)
			{
				_action = action;
			}

			public void Dispose()
			{
				_action();
			}
		}
	}
}