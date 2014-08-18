using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Storage
{
	public class AsyncStorageTests
	{
		[Theory, AutoMoqData]
		public async Task Get_ShouldReturnCorrectValue(
			[Frozen]Mock<IAsyncDataTableFactory> dataTableFactory,
			AsyncStorage sut,
			Mock<IAsyncDataTable<int, object>> dataTable,
		  int key,
			object expected)
		{
			//arrange
			dataTable.Setup(d => d.Get(key)).ReturnsTask(expected);
			dataTableFactory.Setup(d => d.GetAsyncTable<int, object>()).Returns(dataTable.Object);

			//act
			var actual = await sut.Get<int, object>(key, CancellationToken.None);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public async Task AddOrUpdate_ShouldCallAddOrUpdate(
			[Frozen]Mock<IAsyncDataTableFactory> dataTableFactory,
			AsyncStorage sut,
			Mock<IAsyncDataTable<int, object>> dataTable,
			int key,
			object value)
		{
			//arrange
			var verifiable = dataTable.Setup(d => d.AddOrUpdate(key, value)).ReturnsDefaultTaskVerifiable();
			dataTableFactory.Setup(d => d.GetAsyncTable<int, object>()).Returns(dataTable.Object);

			//act
			await sut.AddOrUpdate(key, value, CancellationToken.None);

			//assert
			verifiable.Verify();
		}

		[Theory, AutoMoqData]
		public async Task Delete_ShouldCallAddOrUpdate(
			[Frozen]Mock<IAsyncDataTableFactory> dataTableFactory,
			AsyncStorage sut,
			Mock<IAsyncDataTable<int, object>> dataTable,
			int key,
			object value)
		{
			//arrange
			var verifiable = dataTable.Setup(d => d.Delete(key)).ReturnsDefaultTaskVerifiable();
			dataTableFactory.Setup(d => d.GetAsyncTable<int, object>()).Returns(dataTable.Object);

			//act
			await sut.Delete<int, object>(key, CancellationToken.None);

			//assert
			verifiable.Verify();
		}

		[Theory,
		InlineAutoMoqData(true),
		InlineAutoMoqData(false)]
		public async Task Contains_ShouldReturnCorrectValue(
			bool expected,
			[Frozen]Mock<IAsyncDataTableFactory> dataTableFactory,
			AsyncStorage sut,
			Mock<IAsyncDataTable<int, object>> dataTable,
			int key
			)
		{
			//arrange
			dataTable.Setup(d => d.Contains(key)).ReturnsTask(expected);
			dataTableFactory.Setup(d => d.GetAsyncTable<int, object>()).Returns(dataTable.Object);

			//act
			var actual = await sut.Contains<int, object>(key, CancellationToken.None);

			//assert
			actual.Should().Be(expected);
		}
	}
}
