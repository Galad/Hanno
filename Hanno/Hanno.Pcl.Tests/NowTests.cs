using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Ploeh.Albedo;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests
{
	public class NowContextTests
	{
		[Theory, AutoData]
		public void Current_ShouldBeUtcNow()
		{
			NowContext.Current.Should().BeOfType<SystemUtcNow>();
		}

		[Theory, AutoMoqData]
		public void SetContext_ShouldSetContext(INow expected)
		{
			//act
			NowContext.SetContext(expected);

			//assert
			NowContext.Current.Should().Be(expected);
		}

		[Fact]
		public void SetContext_GuardClause()
		{
			Assert.Throws<ArgumentNullException>(() => NowContext.SetContext(null));
		}
	}

	public class FuncNowTests
	{
		[Theory, AutoData]
		public void Sut_ShouldBeINow(FuncNow sut)
		{
			sut.Should().BeAssignableTo<INow>();
		}

		[Theory, AutoData]
		public void Now_ShouldReturnCorrectValue(DateTimeOffset expected)
		{
			//arrange
			var sut = new FuncNow(() => expected);

			//act
			var actual = sut.Now;

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoData]
		public void Sut_ConstructorGuardClause(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<FuncNow>();
		}
	} 
}
