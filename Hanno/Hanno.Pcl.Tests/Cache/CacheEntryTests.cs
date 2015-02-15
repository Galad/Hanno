using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Cache;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Cache
{
	public class CacheEntryTests
	{
		[Theory, AutoMoqData]
		public void Sut_VerifyGuardClauses(Fixture fixture)
		{
			var assertion = new GuardClauseAssertion(
				fixture,
				new SkipSpecifiedParametersBehaviorExpectation(
				new CompositeBehaviorExpectation(
					new EmptyGuidBehaviorExpectation(),
					new NullReferenceBehaviorExpectation()),
				"value")
				);
			assertion.VerifyConstructors<CacheEntry<string>>();
		}

		[Theory, AutoData]
		public void Id_ShouldReturnCorrectValue(
			[Frozen]Guid id,
			CacheEntry<object> sut)
		{
			//assert
			sut.Id.Should().Be(id);
		}

		[Theory, AutoData]
		public void CacheKey_ShouldReturnCorrectValue(
			Guid id,
			string cacheKey,
			Dictionary<string, string> attributes,
			object value,
			DateTimeOffset date)
		{
			//arrange
			var sut = new CacheEntry<object>(id, cacheKey, attributes, date, value);

			//assert
			sut.CacheKey.Should().Be(cacheKey);
		}

		[Theory, AutoData]
		public void Attributes_ShouldReturnCorrectValue(
			Guid id,
			string cacheKey,
			Dictionary<string,string> attributes,
			object value,
			DateTimeOffset date)
		{
			//arrange
			var sut = new CacheEntry<object>(id, cacheKey, attributes, date, value);

			//assert
			sut.Attributes.Should().Equal(attributes);
		}

		[Theory, AutoData]
		public void Value_ShouldReturnCorrectValue(
			Guid id,
			string cacheKey,
			Dictionary<string, string> attributes,
			object value,
			DateTimeOffset date)
		{
			//arrange
			var sut = new CacheEntry<object>(id, cacheKey, attributes, date, value);

			//assert
			sut.Value.Should().Be(value);
		}
	}
}
