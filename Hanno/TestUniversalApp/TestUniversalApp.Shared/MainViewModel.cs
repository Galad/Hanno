using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno;
using Hanno.Commands;
using Hanno.Extensions;
using Hanno.MVVM;
using Hanno.Navigation;
using Hanno.Validation;
using Hanno.Validation.Rules;
using Hanno.ViewModels;
using System.Reactive.Linq;

namespace TestUniversalApp
{
	public class MainViewModel : ViewModelBase
	{
		private readonly IEntityBuilder _entityBuilder;

		public MainViewModel(IViewModelServices services, IEntityBuilder entityBuilder) : base(services)
		{
			if (entityBuilder == null) throw new ArgumentNullException("entityBuilder");
			_entityBuilder = entityBuilder;
		}

		public ICommand Navigate
		{
			get
			{
				return this.CommandBuilder()
						   .Execute(ct => this.Services.RequestNavigation.Navigate(ct, ApplicationPages.Second))
						   .ToCommand();
			}
		}

		public IObservableViewModel TestObservable
		{
			get
			{
				return this.OvmBuilderProvider.Get("TestObservable")
						   .Execute(ct => Task.FromResult(DateTime.Now))
						   .RefreshOn(Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)))
						   .ToViewModel();
			}
		}

		public IObservableViewModel TestUpdatable
		{
			get
			{
				var r = new Random();
				return this.OvmBuilderProvider
				           .Get("TestUpdatable")
				           .ExecuteUpdatable(ct => Task.FromResult(new int[] {1, 50, 60}), 0)
				           .UpdateOn(() => Observable.Timer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10)).TakeUntil(DateTimeOffset.Now.AddSeconds(10)).Select(l => r.Next(0, 9999)))
				           .UpdateAction(
					           (i, ints) =>
					           {
						           var ii = r.Next(0, ints.Count - 1);
						           return () => ints.Insert(ii, i);
					           })
				           .ToViewModel();
			}
		}

		public ICommand TestEntityConverter
		{
			get
			{
				return this.CommandBuilderProvider
						   .Get()
						   .Execute(() =>
						   {
							   var a = new ClassA() {Test = "bla"};
							   var e = new EntityConverter();
							   var sw = Stopwatch.StartNew();
							   var b = _entityBuilder.BuildFrom<ClassA, ClassB>(a);
							   sw.Stop();
							   Debug.WriteLine(b.Test);
							   Debug.WriteLine(sw.ElapsedTicks);
						   })
						   .ToCommand();
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			TestObservable.Refresh();
			TestUpdatable.Refresh();
		}
	}
}
