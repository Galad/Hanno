using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Services;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.SettingsService
{
	public class SettingsServiceTests
	{
		[Theory, AutoMoqData]
		public async Task ObserveValue_ValueExists_ShouldReturnCorrectValue(
			[Frozen]Mock<IAsyncStorage> table,
			Services.SettingsService sut,
			string key,
			object expected)
		{
			//arrange
			table.Setup(t => t.Get<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(expected);
			table.Setup(t => t.Contains<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(true);

			//act
			var actual = await sut.ObserveValue<object>(key, () => null).Take(1);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task ObserveValue_ValueDoesNotExists_ShouldReturnDefaultValue(
			[Frozen]Mock<IAsyncStorage> table,
			Services.SettingsService sut,
			string key,
			object expected)
		{
			//arrange
			table.Setup(t => t.Get<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(() => null);
			table.Setup(t => t.Contains<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(false);
			table.Setup(t => t.AddOrUpdate(key, expected, It.IsAny<CancellationToken>())).ReturnsDefaultTask();

			//act
			var actual = await sut.ObserveValue<object>(key, () => expected).Take(1);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task ObserveValue_ValueDoesNotExists_ShouldSaveDefaultValue(
			[Frozen]Mock<IAsyncStorage> table,
			Services.SettingsService sut,
			string key,
			object expected)
		{
			//arrange
			table.Setup(t => t.Contains<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(false);
			var verifiable = table.Setup(t => t.AddOrUpdate(key, expected, It.IsAny<CancellationToken>())).ReturnsDefaultTaskVerifiable();

			//act
			await sut.ObserveValue<object>(key, () => expected).Take(1);

			//assert
			verifiable.Verify();
		}

		[Theory, AutoMoqData]
		public async Task ObserveValue_WhenCallSet_ShouldReturnValue(
			[Frozen]ThrowingTestScheduler scheduler,
			[Frozen]Mock<IAsyncStorage> table,
			Services.SettingsService sut,
			string key,
			object expected)
		{
			//arrange
			table.Setup(t => t.Contains<string, object>(key, It.IsAny<CancellationToken>())).ReturnsTask(false);
			table.Setup(t => t.AddOrUpdate(key, It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsDefaultTask();
			
			//act
			var observer = scheduler.CreateObserver<object>();
			sut.ObserveValue(key, () => default(object)).Skip(1).Subscribe(observer);
			await sut.SetValue(key, expected, CancellationToken.None);
			
			//assert
			observer.Values().Last().Should().Be(expected);
		}


		[Theory, AutoMoqData]
		public async Task SetValue_ShouldCallAddOrUpdate(
			[Frozen]ThrowingTestScheduler scheduler,
			[Frozen]Mock<IAsyncStorage> table,
			Services.SettingsService sut,
			string key,
			object expected)
		{
			//arrange
			var verifiable = table.Setup(t => t.AddOrUpdate(key, expected, It.IsAny<CancellationToken>())).ReturnsDefaultTaskVerifiable();

			//act
			await sut.SetValue(key, expected, CancellationToken.None);

			//assert
			verifiable.Verify();
		}
	}
}
