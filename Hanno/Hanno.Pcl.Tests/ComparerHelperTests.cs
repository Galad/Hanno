using System;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests
{
	public class ComparerHelperTests
	{
		[Theory, AutoData]
		public void Max_ShouldReturnCorrectValue()
		{
			//arrange
			var date1 = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var date2 = new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.Zero);

			//act
			var actual = ComparerHelper.Max(date1, date2);

			//assert
			actual.Should().Be(date2);
		}

		[Theory, AutoData]
		public void Min_ShouldReturnCorrectValue()
		{
			//arrange
			var date1 = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero);
			var date2 = new DateTimeOffset(2001, 1, 1, 1, 1, 1, TimeSpan.Zero);

			//act
			var actual = ComparerHelper.Min(date1, date2);

			//assert
			actual.Should().Be(date1);
		} 
	}
}