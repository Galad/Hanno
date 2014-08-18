using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Storage
{
	//[Trait("Category", "Hello")]
	//public class AsyncFromStringKeyValuePairFilesDataTableTests
	//{
	//	[Theory, AutoMoqData]
	//	public async Task Get_ShouldReturnCorrectValue(
	//		[Frozen]Mock<IDataTable<int, object>> innerTable,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut,
	//		object expected,
	//		int key)
	//	{
	//		//arrange
	//		innerTable.Setup(t => t.Get(key)).Returns(expected);

	//		//act
	//		var actual = await sut.Get(key);

	//		//assert
	//		actual.Should().Be(expected);
	//	}

	//	[Theory, AutoMoqData]
	//	public async Task AddOrUpdate_ShouldCallAddOrUpdate(
	//		[Frozen]Mock<IDataTable<int, object>> innerTable,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut,
	//		object value,
	//		int key)
	//	{
	//		//arrange
			
	//		//act
	//		await sut.AddOrUpdate(key, value);

	//		//assert
	//		innerTable.Verify(t => t.AddOrUpdate(key, value));
	//	}

	//	[Theory, AutoMoqData]
	//	public async Task Delete_ShouldCallAddOrUpdate(
	//		[Frozen]Mock<IDataTable<int, object>> innerTable,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key)
	//	{
	//		//arrange

	//		//act
	//		await sut.Delete(key);

	//		//assert
	//		innerTable.Verify(t => t.Delete(key));
	//	}

	//	[Theory, AutoMoqData]
	//	public async Task Query_ShouldReturnCorrectValue(
	//		[Frozen]Mock<IDataTable<int, object>> innerTable,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key,
	//		KeyValuePair<int, object>[] expected)
	//	{
	//		//arrange
	//		innerTable.Setup(t => t.Query()).Returns(expected.AsQueryable());

	//		//act
	//		var actual = await sut.Query(query => query);

	//		//assert
	//		actual.ShouldAllBeEquivalentTo(expected);
	//	}

	//	[Theory, AutoMoqData]
	//	public void Query_GuardClause(
	//		GuardClauseAssertion assertion,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut)
	//	{
	//		var method = sut.GetType().GetMethod("Query");
	//		assertion.Verify(method);
	//	}

	//	[Theory, AutoMoqData]
	//	public void Sut_ConstructorGuardClauses(
	//	  GuardClauseAssertion assertion)
	//	{
	//		assertion.VerifyConstructor<AsyncFromStringKeyValuePairFilesDataTable<int, object>>();
	//	}

	//	[Theory, InlineAutoMoqData(true), InlineAutoMoqData(false)]
	//	public async Task Contains_ShouldReturnCorrectValue(
	//		bool expected,
	//		[Frozen]Mock<IDataTable<int, object>> innerTable,
	//		AsyncFromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key)
	//	{
	//		//arrange
	//		innerTable.Setup(t => t.Contains(key)).Returns(expected);

	//		//act
	//		var actual = await sut.Contains(key);

	//		//assert
	//		actual.Should().Be(expected);
	//	}
	//}
}
