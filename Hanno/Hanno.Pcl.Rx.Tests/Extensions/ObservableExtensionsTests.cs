using System.Reactive.Linq;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture.Xunit2;
using Ploeh.AutoFixture;
using Xunit;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Pcl.Rx.Tests.Extensions
{
	public class ObservableExtensionsTests : ReactiveTest
	{
		[Theory, AutoData]
		public void Delta_ShouldReturnCorrectValue(
		  TestScheduler scheduler)
		{
			//arrange
			var source = scheduler.CreateColdObservable(
				OnNext(0, 0),
				OnNext(1, 10),
				OnNext(2, 8),
				OnNext(3, 16),
				OnNext(4, 20),
				OnNext(5, 10));

			//act
			var actual = scheduler.Start(() => source.Delta((previous, current) => previous - current));

			//assert
			var expected = new[] {-10, 2, -8, -4, 10};
			actual.Values().ShouldAllBeEquivalentTo(expected);
		}

		[Theory, AutoData]
		public void Delta_WithOneValue_ShouldReturnCorrectValue(
		  TestScheduler scheduler)
		{
			//arrange
			var source = scheduler.CreateColdObservable(
				OnNext(0, 0));

			//act
			var actual = scheduler.Start(() => source.Delta((previous, current) => previous - current));

			//assert
			actual.Values().Should().BeEmpty();
		}

		[Theory, AutoData]
		public void Delta_WithNoValue_ShouldReturnCorrectValue(
		  TestScheduler scheduler)
		{
			//arrange
			var source = scheduler.CreateColdObservable(
				OnNext(0, 0));

			//act
			var actual = scheduler.Start(() => source.Delta((previous, current) => previous - current));

			//assert
			actual.Values().Should().BeEmpty();
		}
	}
}