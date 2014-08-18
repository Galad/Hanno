using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Diagnostics;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanno.Tests.Diagnostics
{
	public class StopwatchCompareTests
	{
		//[Theory, AutoData]
		public void Compare_ShouldReturnCorrectValue(
		  Generator<int> generator)
		{
			//arrange
			var firstActionTime = generator.First(i => i >= 50 && i <= 100);
			var secondActionTime = generator.First(i => i >= 50 && i <= 100);

			//act
			var actual = PerformanceTime.Compare(() => Thread.Sleep(firstActionTime), () => Thread.Sleep(secondActionTime));

			//assert
			actual.Item1.Milliseconds.Should().BeInRange(firstActionTime, firstActionTime + 5);
			actual.Item2.Milliseconds.Should().BeInRange(secondActionTime, secondActionTime + 5);
		}

		//[Theory, AutoData]
		public async Task CompareAsync_ShouldReturnCorrectValue(
		  Generator<int> generator)
		{
			//arrange
			var firstActionTime = generator.First(i => i >= 50 && i <= 100);
			var secondActionTime = generator.First(i => i >= 50 && i <= 100);

			//act
			var actual = await PerformanceTime.Compare(() => Task.Delay(firstActionTime), () => Task.Delay(secondActionTime));

			//assert
			actual.Item1.Milliseconds.Should().BeInRange(firstActionTime, firstActionTime + 20);
			actual.Item2.Milliseconds.Should().BeInRange(secondActionTime, secondActionTime + 20);
		} 
	}
}