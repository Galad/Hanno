using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Hanno.Controls
{
	public class AppBar : FrameworkElement
	{
		private readonly ApplicationBar _applicationBar;
		private bool _isInitialized;

		public AppBar()
		{
			AppBarIcons = new List<AppBarIconItem>();
			Loaded += AppBar_Loaded;
			_applicationBar = new ApplicationBar();
		}

		void AppBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (!_isInitialized)
			{
				var page = this.FindFirstParentOfType<PhoneApplicationPage>();
				if (page != null)
				{
					var i = 0;
					foreach (var appBarIconItem in AppBarIcons)
					{
						var dataContextBinding = new Binding("DataContext") {Source = this};
						BindingOperations.SetBinding(appBarIconItem, FrameworkElement.DataContextProperty, dataContextBinding);
						_applicationBar.Buttons.Add(appBarIconItem.ApplicationBarIconButton);
						appBarIconItem.Index = i;
						appBarIconItem.VisibilityChanged += appBarIconItem_VisibilityChanged;
						i++;
					}
					page.ApplicationBar = _applicationBar;
				}
				_isInitialized = true;
			}
		}

		void appBarIconItem_VisibilityChanged(object sender, System.EventArgs e)
		{
			var item = (AppBarIconItem)sender;
			if (item.IsVisible)
			{
				_applicationBar.Buttons.Insert(item.Index, item.ApplicationBarIconButton);
			}
			else
			{
				_applicationBar.Buttons.Remove(item.ApplicationBarIconButton);
			}
		}

		public List<AppBarIconItem> AppBarIcons { get; private set; }
	}
}