using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Hanno.Extensions;
using Hanno.ViewModels;
using System.Windows.Input;
using System.Threading;
using Hanno.Navigation;

namespace TestWindowsPhone
{
	public class TestData
	{
		public string Image { get; set; }
		public string Text { get; set; }
	}
	public class SecondViewModel : ViewModelBase
	{
		public SecondViewModel(IViewModelServices services) : base(services)
		{
		}

		public string Test { get { return "Hello world"; } }

		public int I
		{
			get { return GetValue(factory: () => 0); }
			set { SetValue(value); }
		}

		public int J
		{
			get { return GetValue(factory: () => 0); }
			set { SetValue(value); }
		}

		public int K
		{
			get { return GetValue(factory: () => 0); }
			set { SetValue(value); }
		}

		public IObservableProperty<TestData[]> Items
		{
			get
			{
				return this.GetObservableProperty(() =>
					Observable.Return(
				  Enumerable.Range(0, 100)
							.SelectMany(_ => new[]
							  {
								  new TestData(){Image = "http://kbimages1-a.akamaihd.net/Images/b0fd620d-caba-4123-bb18-eb41e1e00a6e/166/300/False/The+Chase.jpg", Text = Guid.NewGuid().ToString()},
								  new TestData(){Image = "http://kbimages1-a.akamaihd.net/Images/ad1ffcd1-ca3a-4a61-8ff9-afb7a21a3f45/166/300/False/The+Troop.jpg", Text = Guid.NewGuid().ToString()},
								  new TestData(){Image = "http://kbimages1-a.akamaihd.net/Images/1f73b7c0-28e6-4284-950c-419fe72d04be/166/300/False/The+Undead+Pool.jpg", Text = Guid.NewGuid().ToString()},
								  new TestData(){Image = "http://kbimages1-a.akamaihd.net/Images/cfdaddf0-f960-4999-98c1-da3c24176bd3/166/300/False/Bark.jpg", Text = Guid.NewGuid().ToString()}
							  })
							.ToArray())
						.Delay(TimeSpan.FromSeconds(2)));
			}
		}

		public ICommand InsertRequestAndGoBack
		{
			get
			{
				return this.CommandBuilder()
					.Execute(() => Observable.FromAsync(() =>
					{
						this.Services.NavigationService.History.InsertRequest(1, new NavigationRequest(ApplicationPages.Third, new Dictionary<string, string>()));
						return Task.FromResult(true);
						//return this.Services.NavigationService.NavigateBack();
					}))
					.ToCommand();
			}
		}

		public ICommand GoBack
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack2
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack3
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack4
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack5
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack6
		{
			get
			{
				return this.CommandBuilder()
						.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
						.ToCommand();
			}
		}

		public ICommand GoBack7
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack8
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}

		public ICommand GoBack9
		{
			get
			{
				return this.CommandBuilder()
							.Execute(() => Observable.FromAsync(() => this.Services.NavigationService.NavigateBack(CancellationToken.None)))
							.ToCommand();
			}
		}
	}
}
