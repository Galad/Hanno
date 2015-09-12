using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Hanno.Tests.ViewModels
{
	public class ObservableViewModelBuilderOptionsTests : ReactiveTest
	{
		[Theory, AutoMoqData]
		public void EmptyPredicate_ShouldReturnCorrectValue(
			ObservableViewModelBuilderOptions<object> sut)
		{
			//arrange

			//act
			var actual = sut.EmptyPredicate(o => false);

			//assert
			actual.Should().Be(sut);
		}

		[Theory, AutoMoqData]
		public void RefreshOn_ShouldReturnCorrectValue(
			ObservableViewModelBuilderOptions<object> sut)
		{
			//arrange

			//act
			var actual = sut.RefreshOn(Observable.Empty<object>());

			//assert
			actual.Should().Be(sut);
		}

		[Theory, AutoMoqData]
		public void ToViewModel_ShouldReturnCorrectValue(
			Task<object> value,
			TestSchedulers scheduler)
		{
			//arrange
			var sut = new ObservableViewModelBuilderOptions<object>(model => { }, ct => value, scheduler);

			//act
			var actual = sut.ToViewModel();

			//assert
			actual.Should().BeOfType<ObservableViewModel<object>>()
				  .And.Match<ObservableViewModel<object>>(model => model.Source(CancellationToken.None) == value);
		}

		[Theory, AutoMoqData]
		public void RefreshOn_SetRefreshOn(
			ThrowingTestScheduler scheduler,
			object refreshValue,
			ObservableViewModelBuilderOptions<object> sut)
		{
			//arrange
			var refreshOn = new Subject<object>();

			scheduler.Schedule(TimeSpan.FromTicks(201), () => refreshOn.OnNext(refreshValue));

			//act
			var actual = sut.RefreshOn(refreshOn)
							.ToViewModel();
			var result = scheduler.Start(() => actual.As<ObservableViewModel<object>>().RefreshOn);

			//assert
			result.Values().Should().HaveCount(1);
		}

		[Theory, AutoMoqData]
		public void EmptyPredicate_ShouldSetEmptyPredicate(
			ObservableViewModelBuilderOptions<object> sut)
		{
			//arrange
			Func<object, bool> emptyPredicate = o => false;

			//act
			var actual = sut.EmptyPredicate(emptyPredicate)
							.ToViewModel();

			//assert
			actual.As<ObservableViewModel<object>>().EmptyPredicate.Should().Be(emptyPredicate);
		}

		[Theory, AutoMoqData]
		public void ToViewModel_ShouldSaveViewModel(
			Task<object> source,
			TestSchedulers scheduler)
		{
			//arrange
			IObservableViewModel viewModel = null;
			var sut = new ObservableViewModelBuilderOptions<object>(model => viewModel = model, ct => source, scheduler);

			//act
			var actual = sut.ToViewModel();

			//assert
			actual.Should().Be(viewModel.As<ObservableViewModel<object>>());
		}


		[Theory, AutoMoqData]
		public void Timeout_ShouldSetTimeout(
				ObservableViewModelBuilderOptions<object> sut,
				TimeSpan expected)
		{
			//arrange

			//act
			var actual = sut.Timeout(expected).ToViewModel();

			//assert
			actual.Should().BeOfType<ObservableViewModel<object>>()
			      .And.Match<ObservableViewModel<object>>(model => model.TimeoutDelay == expected);
		}
	}
}