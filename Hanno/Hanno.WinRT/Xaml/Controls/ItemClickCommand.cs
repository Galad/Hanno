using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hanno.Xaml.Controls
{
	public class ItemClickCommand
	{
		private static ConditionalWeakTable<ListViewBase, ItemClickEventHandler> _eventHandlers;

		static ItemClickCommand()
		{
			_eventHandlers = new ConditionalWeakTable<ListViewBase, ItemClickEventHandler>();
		}

		public static ICommand GetCommand(ListViewBase obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(ListViewBase obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandChanged));

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var listView = (ListViewBase)d;
			if (listView == null)
			{
				return;
			}
			if (!listView.IsItemClickEnabled)
			{
				Debug.WriteLine("ItemClick should be enabled in order to use the command");
				Debugger.Break();
			}
			ClearPreviousEvents(listView);
			if (e.NewValue == null)
			{
				return;
			}
			
			var eventHandler = new ItemClickEventHandler(ListViewBase_ItemClick);
			_eventHandlers.Add(listView, eventHandler);
			listView.ItemClick += eventHandler;
		}

		private static void ListViewBase_ItemClick(object sender, ItemClickEventArgs e)
		{
			HandleItemClick((ListViewBase)sender, e.ClickedItem);
		}


		private static void HandleItemClick(ListViewBase listView, object item)
		{
			var command = GetCommand(listView);

			if (command == null || !command.CanExecute(item))
			{
				return;
			}
			command.Execute(item);
		}

		private static void ClearPreviousEvents(ListViewBase listView)
		{
			ItemClickEventHandler eventHandler;
			if (_eventHandlers.TryGetValue(listView, out eventHandler))
			{
				listView.ItemClick -= eventHandler;
				_eventHandlers.Remove(listView);
			}
		}
	}
}