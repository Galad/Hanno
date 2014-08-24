using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	public class UpdatableViewModelTests : ReactiveTest
	{
		[Theory, AutoData]
		public async Task ViewModel_ShouldReturnCorrectValue(
		  IFixture fixture,
			TestSchedulers scheduler)
		{
			//arrange
			const int insertionIndex = 5;
			var initialList = fixture.CreateMany<int>(10).ToArray();
			var addedList = fixture.CreateMany<int>(10).ToArray();
			var expected = initialList.Take(insertionIndex)
									  .Concat(addedList.Reverse())
									  .Concat(initialList.Skip(insertionIndex))
									  .ToArray();
			var notifications = scheduler.CreateColdObservable(addedList.Select((i, ii) => OnNext(Subscribed + 1 + ii, i)).ToArray());
			var sut = new UpdatableObservableViewModelBuilderOptions<int, int[], int>(
				_ => { },
				ct => Task.FromResult(initialList),
				() => notifications,
				scheduler,
				scheduler,
				scheduler)
				.UpdateAction((i, o) => () => o.Insert(insertionIndex, i))
				.ToViewModel();

			//act

			scheduler.Start();
			await sut.RefreshAsync();
			scheduler.AdvanceBy(Disposed);
			var actual = ((IObservableViewModel<ObservableCollection<int>>)sut).CurrentValue;
			//assert

			actual.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, AutoData]
		public async Task ViewModel_WhenRefreshed_ShouldDisposePreviousNotificationsSubscriptions(
		  IFixture fixture,
			TestSchedulers scheduler,
			int[] values)
		{
			//arrange
			var notifications = scheduler.CreateColdObservable<int>();
			var sut = new UpdatableObservableViewModelBuilderOptions<int, int[], int>(
				_ => { },
				ct => Task.FromResult(values),
				() => notifications,
				scheduler,
				scheduler,
				scheduler)
				.UpdateAction((i, o) => () => { })
				.ToViewModel();
			const long disposeTime = 805;

			//act

			scheduler.Start();
			await sut.RefreshAsync();
			//we advance to an arbitrary time
			scheduler.AdvanceBy(disposeTime);
			//the subscription to the new observable should happen here
			//the first subscription should be dispose at the current scheduler time
			await sut.RefreshAsync();

			//assert
			notifications.Subscriptions[0].Unsubscribe.Should().Be(disposeTime);
		}

		[Theory,
		AutoData]
		public void ViewModel_WhenInitialized_WithRefreshOnCollectionUpdateNotification_ShouldRefresh(
			IFixture fixture,
			TestSchedulers scheduler,
			int[] expectedValue)
		{
			//arrange
			var notifications = scheduler.CreateColdObservable(OnNext(200, 1));
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			var sut = new UpdatableObservableViewModelBuilderOptions<int, int[], int>(
				_ => { },
				ct => Task.FromResult(expectedValue),
				() => notifications,
				scheduler,
				scheduler,
				scheduler)
				.UpdateAction((i, o) => () => { })
				.RefreshOnCollectionUpdateNotification()
				.ToViewModel();
			sut.Subscribe(observer);

			//act
			scheduler.Start();

			//assert
			var expected = new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Value,
				Value = expectedValue
			};
			observer.Values().Last().Value.As<ObservableCollection<int>>().ShouldAllBeEquivalentTo(expectedValue);
		}

		[Theory,
		AutoData]
		public async Task ViewModel_WhenEmpty_WithRefreshOnCollectionUpdateNotification_ShouldRefresh(
			IFixture fixture,
			TestSchedulers scheduler,
			int[] expectedValue)
		{
			//arrange
			var notifications = scheduler.CreateColdObservable(OnNext(200, 1));
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			var count = 0;
			var sut = new UpdatableObservableViewModelBuilderOptions<int, int[], int>(
				_ => { },
				ct => Task.FromResult(expectedValue),
				() => notifications,
				scheduler,
				scheduler,
				scheduler)
				.UpdateAction((i, o) => () => { })
				.RefreshOnCollectionUpdateNotification()
				.EmptyPredicate(_ => ++count == 1)
				.ToViewModel();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();
			scheduler.AdvanceBy(300);

			//assert
			observer.Values().Last().Value.As<ObservableCollection<int>>().ShouldAllBeEquivalentTo(expectedValue);
		}

		[Theory,
		AutoData]
		public async Task ViewModel_WhenError_WithRefreshOnCollectionUpdateNotification_ShouldRefresh(
			IFixture fixture,
			TestSchedulers scheduler,
			int[] expectedValue)
		{
			//arrange
			var notifications = scheduler.CreateColdObservable(OnNext(200, 1));
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			var count = 0;
			var sut = new UpdatableObservableViewModelBuilderOptions<int, int[], int>(
				_ => { },
				async ct =>
				{
					if (++count == 1)
						throw new Exception();
					else
						return expectedValue;
				},
				() => notifications,
				scheduler,
				scheduler,
				scheduler)
				.UpdateAction((i, o) => () => { })
				.RefreshOnCollectionUpdateNotification()
				.ToViewModel();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();
			scheduler.AdvanceBy(300);

			//assert
			observer.Values().Last().Value.As<ObservableCollection<int>>().ShouldAllBeEquivalentTo(expectedValue);
		}
	}
}