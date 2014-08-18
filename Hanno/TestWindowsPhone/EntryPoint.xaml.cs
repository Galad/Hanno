using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Hanno.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TestWindowsPhone
{
	public partial class EntryPoint : PhoneApplicationPage
	{
		public EntryPoint()
		{
			InitializeComponent();
			((App)App.Current).RequestNavigation.Navigate(CancellationToken.None, ApplicationPages.Main);
			((App)App.Current).RequestNavigation.Navigate(CancellationToken.None, ApplicationPages.Second);
		}
	}
}