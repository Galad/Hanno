using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Cache;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Cache
{
	public class MemoryCacheEntryRepositoryTests
	{
		[Theory, AutoData]
		public void Sut_IsCacheEntryRepository(MemoryCacheEntryRepository sut)
		{
			sut.Should().BeAssignableTo<ICacheEntryRepository>();
		}

		[Theory, AutoData]
		public void Sut_VerifyGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<MemoryCacheEntryRepository>();
		}

		[Theory, AutoData]
		public async Task Get_WhenValueDoesNotExists_ShouldReturnCorrectValue(
			MemoryCacheEntryRepository sut,
			string cacheKey,
			Dictionary<string, string> attributes)
		{
			//arrange

			//act
			var actual = await sut.Get<object>(CancellationToken.None, cacheKey, attributes);

			//assert
			actual.Should().BeNull();
		}

		[Theory, AutoMoqData]
		public async Task Get_WhenValueExists_ShouldReturnCorrectValue(
			MemoryCacheEntryRepository sut,
			[Frozen]Dictionary<string, string> attributes,
			[Frozen]string cacheKey,
			[Frozen]Guid id,
			CacheEntry<object> expected)
		{
			//arrange
			await sut.AddOrUpdate(CancellationToken.None, expected);

			//act
			var actual = await sut.Get<object>(CancellationToken.None, expected.CacheKey, expected.Attributes);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task Get_WhenValueIsUpdated_ShouldReturnCorrectValue(
			MemoryCacheEntryRepository sut,
			CacheEntry<object> cacheEntry,
			object newValue)
		{
			//arrange
			var expected = new CacheEntry<object>(cacheEntry.Id, cacheEntry.CacheKey, cacheEntry.Attributes, DateTimeOffset.Now, newValue);
			await sut.AddOrUpdate(CancellationToken.None, cacheEntry);
			await sut.AddOrUpdate(CancellationToken.None, expected);

			//act
			var actual = await sut.Get<object>(CancellationToken.None, expected.CacheKey, expected.Attributes);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task Get_AfterRemoving_ShouldReturnCorrectValue(
		MemoryCacheEntryRepository sut,
		CacheEntry<object> cacheEntry,
		object newValue)
		{
			//arrange			
			await sut.AddOrUpdate(CancellationToken.None, cacheEntry);
			await sut.Remove<CacheEntry<object>>(CancellationToken.None, cacheEntry.CacheKey, cacheEntry.Id);

			//act
			var actual = await sut.Get<object>(CancellationToken.None, cacheEntry.CacheKey, cacheEntry.Attributes);

			//assert
			actual.Should().BeNull();
		}
	}
}
