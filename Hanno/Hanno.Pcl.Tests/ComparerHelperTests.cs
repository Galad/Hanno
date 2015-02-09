using System;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;
using System.Linq;

namespace Hanno.Tests
{
	public class ComparerHelperTests
	{
		[Theory,
		InlineData(2000, 2001, 2001),
		InlineData(2001, 2000, 2001),
		InlineData(2000, 2000, 2000),
		]
		public void Max_ShouldReturnCorrectValue(
			int year1,
			int year2,
			int expectedYear)
		{
			//arrange
			var date1 = new DateTimeOffset(year1, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var date2 = new DateTimeOffset(year2, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var expected = new[] { date1, date2 }.First(d => d.Year == expectedYear);

			//act
			var actual = ComparerHelper.Max(date1, date2);

			//assert
			actual.Should().Be(expected);
		}

		[Theory,
		InlineData(2000, 2001, 2000),
		InlineData(2001, 2000, 2000),
		InlineData(2000, 2000, 2000),
		]
		public void Min_ShouldReturnCorrectValue(
			int year1, 
			int year2, 
			int expectedYear)
		{
			//arrange
			var date1 = new DateTimeOffset(year1, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var date2 = new DateTimeOffset(year2, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var expected = new[] { date1, date2 }.First(d => d.Year == expectedYear);

			//act
			var actual = ComparerHelper.Min(date1, date2);

			//assert
			actual.Should().Be(expected);
		}
	}
}