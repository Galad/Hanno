using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Cache;
using Hanno.CqrsInfrastructure;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Ploeh.AutoFixture;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.Cache
{
	#region Customization
	public class CacheServiceCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Customize<Mock<ICacheEntryRepository>>(c => c.Do(mock =>
			{
				mock.Setup(r => r.AddOrUpdate(CancellationToken.None, It.IsAny<CacheEntry<CacheServiceTests.TestCacheReturnValue>>()))
					.ReturnsDefaultTask();
				mock.Setup(r => r.Remove<CacheServiceTests.TestCacheReturnValue>(CancellationToken.None, It.IsAny<string>(), It.IsAny<Guid>()))
					.ReturnsDefaultTask();
				mock.Setup(r => r.Get<CacheServiceTests.TestCacheReturnValue>(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
					.ReturnsDefaultTask();
			}));
			fixture.Freeze<DateTimeOffset>();			            
		}
	}

	public class CacheServiceCompositeCustomization : CompositeCustomization
	{
		public CacheServiceCompositeCustomization()
			: base(new HannoCustomization(), new CacheServiceCustomization())
		{
		}
	}

	public class CacheServiceAutoDataAttribute : AutoDataAttribute
	{
		public CacheServiceAutoDataAttribute()
			: base(new Fixture().Customize(new CacheServiceCompositeCustomization()))
		{
		}
	}

	public class CacheServiceInlineAutoDataAttribute : CompositeDataAttribute
	{
		public CacheServiceInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new CacheServiceAutoDataAttribute())
		{
		}
	}
	#endregion

	public class CacheServiceTests
	{
		public class TestCacheReturnValue
		{
			public string Value1 { get; set; }
			public int Value2 { get; set; }
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeCacheService(
			CacheService sut)
		{
			sut.Should().BeAssignableTo<ICacheService>();
		}

		[Theory, AutoMoqData]
		public void Sut_TestGuardClauses(
			GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<CacheService>();
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsNotCached_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(default(CacheEntry<TestCacheReturnValue>));

			//act
			var actual = await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.Zero);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsNotCached_ShouldAddEntryToCache(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(default(CacheEntry<TestCacheReturnValue>));
			cacheEntryRepo.Setup(c => c.AddOrUpdate(CancellationToken.None, It.Is<CacheEntry<TestCacheReturnValue>>(entry =>
				entry.CacheKey == key && entry.Value == expected)))
			              .ReturnsDefaultTask()
			              .Verifiable();

			//act
			await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.Zero);

			//assert
			cacheEntryRepo.Verify();
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCached_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(Guid.NewGuid(), key, new Dictionary<string, string>(), now, expected));

			//act
			var actual = await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(notExpected), TimeSpan.Zero);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCachedAndAgeEntryIsTooOld_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now)
		{
			//arrange			
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(
				              Guid.NewGuid(),
				              key,
				              new Dictionary<string, string>(),
				              now.Subtract(TimeSpan.FromMinutes(10)),
				              notExpected));

			//act
			var actual = await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.FromMinutes(5));

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCachedAndAgeEntryIsTooOld_ShouldRemoveOldEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now,
			Guid id)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(
				              id,
				              key,
				              new Dictionary<string, string>(),
				              now.Subtract(TimeSpan.FromMinutes(10)),
				              notExpected));

			//act
			await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.FromMinutes(5));

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(CancellationToken.None, key, id));
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCachedAndAgeEntryIsTooOld_ShouldAddNewEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now,
			Guid id)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(
				              id,
				              key,
				              new Dictionary<string, string>(),
				              now.Subtract(TimeSpan.FromMinutes(10)),
				              notExpected));
			cacheEntryRepo.Setup(c => c.AddOrUpdate(CancellationToken.None, It.Is<CacheEntry<TestCacheReturnValue>>(entry =>
				entry.CacheKey == key &&
				entry.Value == expected &&
				entry.Attributes.Count == 0)))
			              .ReturnsDefaultTask()
			              .Verifiable();

			//act
			await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.FromMinutes(5));

			//assert
			cacheEntryRepo.Verify();
		}

		#region With attributes

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsNotCachedWithAttributes_ShouldAddEntryToCache(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			Dictionary<string, string> attributes)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, attributes))
			              .ReturnsTask(default(CacheEntry<TestCacheReturnValue>));
			cacheEntryRepo.Setup(c => c.AddOrUpdate(CancellationToken.None, It.Is<CacheEntry<TestCacheReturnValue>>(entry =>
				entry.CacheKey == key && entry.Value == expected && entry.Attributes == attributes)))
			              .ReturnsDefaultTask()
			              .Verifiable();

			//act
			await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.Zero, attributes);

			//assert
			cacheEntryRepo.Verify();
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCachedWithAttributes_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now,
			Dictionary<string, string> attributes)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, attributes))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(Guid.NewGuid(), key, attributes, now, expected));

			//act
			var actual = await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(notExpected), TimeSpan.Zero, attributes);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsCachedAndAgeEntryIsTooOldWithAttributes_ShouldAddNewEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			DateTimeOffset now,
			Guid id,
			Dictionary<string, string> attributes)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, attributes))
			              .ReturnsTask(new CacheEntry<TestCacheReturnValue>(
				              id,
				              key,
				              new Dictionary<string, string>(),
				              now.Subtract(TimeSpan.FromMinutes(10)),
				              notExpected));
			cacheEntryRepo.Setup(c => c.AddOrUpdate(CancellationToken.None, It.Is<CacheEntry<TestCacheReturnValue>>(entry =>
				entry.CacheKey == key &&
				entry.Value == expected &&
				entry.Attributes == attributes)))
			              .ReturnsDefaultTask()
			              .Verifiable();

			//act
			await sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(expected), TimeSpan.FromMinutes(5), attributes);

			//assert
			cacheEntryRepo.Verify();
		}

		#endregion

		#region With concurrency

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsNotCachedWithConcurrency_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(default(CacheEntry<TestCacheReturnValue>));


			//act
			Task<TestCacheReturnValue> actualTask = null;
			await sut.ExecuteWithCache(CancellationToken.None, key, () =>
			{
				actualTask = sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(notExpected), TimeSpan.Zero);
				return Task.FromResult(expected);
			}, TimeSpan.Zero);
			var actual = await actualTask;

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CacheServiceAutoData]
		public async Task ExecuteWithCache_WhenValueIsNotCachedWithConcurrencyAndAttributes_ShouldReturnCorrectValue(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue expected,
			TestCacheReturnValue notExpected,
			Dictionary<string, string> attributes)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, attributes))
			              .ReturnsTask(default(CacheEntry<TestCacheReturnValue>));


			//act
			Task<TestCacheReturnValue> actualTask = null;
			await sut.ExecuteWithCache(CancellationToken.None, key, () =>
			{
				actualTask = sut.ExecuteWithCache(CancellationToken.None, key, () => Task.FromResult(notExpected), TimeSpan.Zero, attributes);
				return Task.FromResult(expected);
			}, TimeSpan.Zero, attributes);
			var actual = await actualTask;

			//assert
			actual.Should().Be(expected);
		}

		#endregion

		[Theory, CacheServiceAutoData]
		public async Task Invalidate_ShouldRemoveCacheEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			CacheEntry<TestCacheReturnValue> entry)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(entry);

			//act
			await sut.Invalidate<TestCacheReturnValue>(CancellationToken.None, key);

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(CancellationToken.None, key, entry.Id));
		}

		[Theory, CacheServiceAutoData]
		public async Task Invalidate_WithAttributes_ShouldRemoveCacheEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			CacheEntry<TestCacheReturnValue> entry,
			Dictionary<string,string> attributes)
		{
			//arrange
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, attributes))
			              .ReturnsTask(entry);

			//act
			await sut.Invalidate<TestCacheReturnValue>(CancellationToken.None, key, attributes);

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(CancellationToken.None, key, entry.Id));
		}

		[Theory, CacheServiceAutoData]
		public async Task Invalidate_WithEntryOlderThanMinAge_ShouldRemoveCacheEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue value,
			DateTimeOffset now)
		{
			//arrange
			var entry = new CacheEntry<TestCacheReturnValue>(Guid.NewGuid(), key, new Dictionary<string, string>(), now.Subtract(TimeSpan.FromMinutes(10)), value);
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
						  .ReturnsTask(entry);

			//act
			await sut.Invalidate<TestCacheReturnValue>(CancellationToken.None, key, TimeSpan.FromMinutes(5));

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(CancellationToken.None, key, entry.Id));
		}

		[Theory, CacheServiceAutoData]
		public async Task Invalidate_WithEntryYoungerThanMinAge_ShouldNotRemoveCacheEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue value,
			DateTimeOffset now)
		{
			//arrange
			var entry = new CacheEntry<TestCacheReturnValue>(Guid.NewGuid(), key, new Dictionary<string, string>(), now.Subtract(TimeSpan.FromMinutes(2)), value);
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
						  .ReturnsTask(entry);

			//act
			await sut.Invalidate<TestCacheReturnValue>(CancellationToken.None, key, TimeSpan.FromMinutes(5));

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(CancellationToken.None, key, entry.Id), Times.Never());
		}


		[Theory, CacheServiceAutoData]
		public async Task Invalidate_WithIsYoungerThanMinAge_ShouldNotRemoveCacheEntry(
			[Frozen] Mock<ICacheEntryRepository> cacheEntryRepo,
			CacheService sut,
			string key,
			TestCacheReturnValue value,
			DateTimeOffset now)
		{
			//arrange
			var entry = new CacheEntry<TestCacheReturnValue>(Guid.NewGuid(), key, new Dictionary<string, string>(), now.Subtract(TimeSpan.FromMinutes(2)), value);
			cacheEntryRepo.Setup(c => c.Get<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, It.Is<IDictionary<string, string>>(d => d.Count == 0)))
			              .ReturnsTask(entry);

			//act
			await sut.Invalidate<TestCacheReturnValue>(CancellationToken.None, key, TimeSpan.FromMinutes(2));

			//assert
			cacheEntryRepo.Verify(c => c.Remove<TestCacheReturnValue>(It.IsAny<CancellationToken>(), key, entry.Id), Times.Never());
		}
	}
}