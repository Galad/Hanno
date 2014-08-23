using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests
{
	public class EnumHelperTests
	{
		public enum EnumTest
		{
			Value1,
			Value2,
			Value3
		}

		[Fact]
		public void GetValues_ShouldReturnCorrectValues()
		{
			//act
			var actual = EnumHelper.GetValues<EnumTest>();

			var expected = new EnumTest[] {EnumTest.Value1, EnumTest.Value2, EnumTest.Value3};
			actual.ShouldAllBeEquivalentTo(expected);
		}
	}
}