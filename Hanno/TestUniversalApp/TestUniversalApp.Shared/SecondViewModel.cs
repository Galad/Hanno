using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Hanno.Extensions;
using Hanno.Navigation;
using Hanno.ViewModels;

namespace TestUniversalApp
{
	public class SecondViewModel : ViewModelBase
	{
		public ICommand Navigate
		{
			get
			{
				return this.CommandBuilder()
						   .Execute(ct => this.Services.RequestNavigation.Navigate(ct, ApplicationPages.Third))
						   .ToCommand();
			}
		}
	}
}
