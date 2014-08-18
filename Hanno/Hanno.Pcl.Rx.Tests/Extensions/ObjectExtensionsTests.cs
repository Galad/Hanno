using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using FluentAssertions;
using Hanno.Rx.Extensions;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Pcl.Rx.Tests.Extensions
{
	public class ObjectExtensionsTests : ReactiveTest
	{
		[Fact]
		public void Convert_ShouldConvert()
		{
			IObservable<string> observableSource = Observable.Return("Test");
			object observableObject = observableSource;
			IObservable<object> result = observableObject.Convert<object>();
			result.Should().NotBeNull();
		}

		[Theory, AutoData]
		public void Convert_ObservableShouldReturnValue(ThrowingTestScheduler scheduler)
		{
			string expected = "Test";
			IObservable<string> observableSource = Observable.Return(expected);
			object observableObject = observableSource;
			IObservable<object> result = observableObject.Convert<object>();
			var actual = scheduler.Start(() => result).Values().First();
			actual.Should().Be(expected);
		}


		[Theory, AutoMoqData]
		public void AsObservableCollectionWithNotifications_ShouldReturnObservableCollection(
			 IEnumerable<object> sut,
			 IObservable<object> notifications,
			 ThrowingTestScheduler scheduler)
		{
			//arrange
			ObservableCollection<object> observableCollection;

			//act
			sut.AsObservableCollectionWithNotifications(notifications, (o, objects) => () => { }, scheduler, scheduler, out observableCollection);

			//assert
			observableCollection.ShouldAllBeEquivalentTo(sut);
		}

		[Theory, AutoMoqData]
		public void AsObservableCollectionWithNotifications_ShouldExecuteActionsOnNotifications(
			 IEnumerable<object> sut,
			 object[] itemsToAdd,
			 ThrowingTestScheduler scheduler)
		{
			//arrange
			var notifications = scheduler.CreateColdObservable(
				itemsToAdd.Select((item, i) => OnNext(Subscribed + i + 1, item))
				          .ToArray());
			ObservableCollection<object> observableCollection = null;

			//act
			scheduler.Start(() =>
				sut.AsObservableCollectionWithNotifications(
					notifications,
					(o, objects) => () => objects.Add(o),
					scheduler,
					scheduler,
					out observableCollection));
			var expected = sut.Concat(itemsToAdd);

			//assert
			observableCollection.ShouldAllBeEquivalentTo(expected);
		}
	}
}