using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Cache;
using Hanno.CqrsInfrastructure;
using Hanno.Extensions;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Ploeh.AutoFixture;
using Xunit;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.Cache
{
	public class CacheAsyncQueryBusTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsAsyncQueryBus(
		  CacheAsyncQueryBus sut)
		{
			sut.IsAssignableTo<IAsyncQueryBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_TestGuardClauses(
		  GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<CacheAsyncQueryBus>();
		}

		[Theory, AutoMoqData]
		public async Task ProcessQuery_QueryIsNotCacheInfo_ShouldReturnCorrectValue(
			[Frozen]Mock<IAsyncQueryBus> queryBus,
			IAsyncQuery<object> query,
			object expected,
			CacheAsyncQueryBus sut)
		{
			//arrange
			queryBus.Setup(q => q.ProcessQuery<IAsyncQuery<object>, object>(query)).ReturnsTask(expected);

			//act
			var actual = await sut.ProcessQuery<IAsyncQuery<object>, object>(query);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task ProcessQuery_QueryIsCacheInfo_ShouldReturnCorrectValue(
			[Frozen]TimeSpan defaultAge,
			[Frozen]Mock<ICacheService> cacheService,
			CacheAsyncQueryBus sut,
			Mock<IAsyncQuery<object>> query,
			TimeSpan cacheAge,
			string key,
			IDictionary<string, string> attributes,
			object expected,
			object notExpected)
		{
			//arrange
			query.As<ICacheInfos>().Setup(c => c.Attributes).Returns(attributes);
			query.As<ICacheInfos>().Setup(c => c.Key).Returns(key);
			query.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
			cacheService.Setup(c => c.ExecuteWithCache(It.IsAny<CancellationToken>(), key, It.IsAny<Func<Task<object>>>(), defaultAge, attributes))
						.ReturnsTask(expected);

			//act
			var actual = await sut.ProcessQuery<IAsyncQuery<object>, object>(query.Object);

			//assert
			actual.Should().Be(expected);
		}
	}
}