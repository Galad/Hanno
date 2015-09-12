using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Extensions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.ViewModels
{
	public class ViewModelTest : ViewModelBase
	{
		public ViewModelTest(IViewModelServices services) : base(services)
		{
		}
	}

	public class ViewModelExtensionsTests : ReactiveTest
	{
		[Theory, ViewModelBaseAutoData]
		public void ObservableFromPool_ShouldReturnCorrectValue(
			ViewModelTest viewModel,
			TestScheduler scheduler)
		{
			//arrange
			var observable = scheduler.CreateColdObservable(
				OnNext(200, 1),
				OnNext(201, 2),
				OnNext(210, 3));
			
			//act
			var actual = scheduler.Start(() =>
			{
				var obs = viewModel.ObservableFromPool(() => observable, scheduler, "Test");
				return obs;
			});

			//assert
			actual.Values().ShouldAllBeEquivalentTo(new[] { 1, 2, 3 });
		}
	}
}
